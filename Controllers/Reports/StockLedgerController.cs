using KerashineERP.Data;
using KerashineERP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerashineERP.Controllers.Reports
{
    public class StockLedgerController : BaseController
    {
        private readonly AppDbContext _context;
        public StockLedgerController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(DateTime? from, DateTime? to, string type="All")
        {
            var companyId = HttpContext.Session.GetInt32("CompanyID") ?? 1;
            var fiscalId = HttpContext.Session.GetInt32("FiscalID") ?? 1;
            
            var vm = new StockLedgerReportVM{
                FromDate = from ?? new DateTime(DateTime.Now.Year,1,1),
                ToDate = to ?? DateTime.Now,
                FilterType = type
            };

            var query = from d in _context.INV_StockTransactionDetail
                        join h in _context.INV_StockTransactionHeader on new { d.CompanyID, d.StockTransactionID } equals new { h.CompanyID, h.StockTransactionID }
                        join dt in _context.SET_DocumentType on new { h.CompanyID, h.DocumentTypeID } equals new { dt.CompanyID, dt.DocumentTypeID }
                        join item in _context.INV_Item on d.ItemID equals item.ItemId
                        where h.CompanyID == companyId && h.StatusCode == 1
                        select new { d, h, dt, item };

            if(type != "All") query = query.Where(x=> x.item.ItemType == type);

            var grouped = await query.GroupBy(x=> new { x.item.ItemId, x.item.ItemCode, x.item.ItemName, x.item.ItemType, x.item.SizeUOM })
            .Select(g=> new StockLedgerItem {
                ItemCode = g.Key.ItemCode,
                ItemName = g.Key.ItemName,
                ItemType = g.Key.ItemType,
                UOM = g.Key.SizeUOM,
                OpeningQty = g.Where(x=> x.dt.DocumentType=="OPENING").Sum(x=> (decimal?)x.d.Quantity)??0,
                InQty = g.Where(x=> x.dt.DocumentType=="ADJUSTMENT_IN" || x.dt.DocumentType=="PURCHASE" || x.dt.DocumentType=="GRN" || x.dt.DocumentType=="PRODUCTION_IN").Sum(x=> (decimal?)x.d.Quantity)??0,
                OutQty = g.Where(x=> x.dt.DocumentType=="ADJUSTMENT_OUT" || x.dt.DocumentType=="TRANSFER").Sum(x=> (decimal?)x.d.Quantity)??0,
                Rate = g.Average(x=> (decimal?)x.d.Rate)??0
            }).ToListAsync();

            foreach(var i in grouped){
                i.ClosingQty = i.OpeningQty + i.InQty - i.OutQty;
                i.Value = i.ClosingQty * i.Rate;
            }
            
            vm.Items = grouped.Where(x=> x.ClosingQty != 0 || x.OpeningQty !=0).ToList();
            return View(vm);
        }
    }
}