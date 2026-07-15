using KerashineERP.Data;
using KerashineERP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerashineERP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Welcome se CompanyID mil gaya
            var companyId = HttpContext.Session.GetInt32("CompanyID") ?? 0;
            var fiscalId = HttpContext.Session.GetInt32("FiscalID") ?? 0;

            // 1. Stock Summary - SET_DocumentType join karke
            var stockSummary = await (
                from d in _context.INV_StockTransactionDetail
                join h in _context.INV_StockTransactionHeader on 
                    new { d.CompanyID, d.StockTransactionID } equals 
                    new { h.CompanyID, h.StockTransactionID }

                join dt in _context.SET_DocumentType on 
                    new { h.CompanyID, h.DocumentTypeID } equals 
                    new { dt.CompanyID, dt.DocumentTypeID }
                where h.StatusCode == 1 && h.CompanyID == companyId && h.FiscalID == fiscalId
                group new { d, dt } by d.ItemID into g
                select new
                {
                    ItemId = g.Key,
                    CurrentStock = g.Sum(x => 
                        x.dt.DocumentType == "OPENING" || 
                        x.dt.DocumentType == "ADJUSTMENT_IN" || 
                        x.dt.DocumentType == "PURCHASE" || 
                        x.dt.DocumentType == "PRODUCTION_IN" ||
                        x.dt.DocumentType == "GRN"
                        ? x.d.Quantity 
                        : -x.d.Quantity),
                    
                    TotalValue = g.Sum(x => 
                        x.dt.DocumentType == "OPENING" || 
                        x.dt.DocumentType == "ADJUSTMENT_IN" || 
                        x.dt.DocumentType == "PURCHASE" || 
                        x.dt.DocumentType == "PRODUCTION_IN" ||
                        x.dt.DocumentType == "GRN"
                        ? x.d.Quantity * x.d.Rate 
                        : -(x.d.Quantity * x.d.Rate))
                }).ToListAsync();

            // 2. Items fetch karo CompanyID filter ke sath
            var activeItems = await _context.INV_Item
                .Where(x => x.CompanyID == companyId && x.IsActive)
                .ToListAsync();

            // 3. C# mein Low Stock calculate karo
            var lowStockItems = (
                from item in activeItems
                join stock in stockSummary on item.ItemId equals stock.ItemId into stockGroup
                from s in stockGroup.DefaultIfEmpty()
                let currentStock = s?.CurrentStock ?? 0
                where item.ReorderLevel > 0 && currentStock <= item.ReorderLevel
                orderby currentStock
                select new ItemStockViewModel
                {
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ItemType = item.ItemType,
                    CurrentStock = currentStock,
                    StockUOM = item.SizeUOM, // Tumhare paas SizeUOM hai
                    ReorderLevel = item.ReorderLevel
                })
                .Take(5)
                .ToList();

            var model = new DashboardViewModel
            {
                TotalItems = activeItems.Count,
                RawMaterialCount = activeItems.Count(x => x.ItemType == "RM"), // Ya "Raw Material"
                PackagingMaterialCount = activeItems.Count(x => x.ItemType == "PM"), // Ya "Packaging Material"
                FinishedGoodsCount = activeItems.Count(x => x.ItemType == "FG"), // Ya "Finished Good"
                TotalStockValue = stockSummary.Sum(x => x.TotalValue),
                LowStockItems = lowStockItems
            };

            return View(model);
        }
    }
}