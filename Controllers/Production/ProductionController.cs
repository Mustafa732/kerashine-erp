using KerashineERP.Data;
using KerashineERP.Models.Common;
using KerashineERP.Models.Production;
using KerashineERP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Controllers;

namespace KerashineERP.Production.Controllers
{
    public class ProductionController : BaseController
    {
        private readonly AppDbContext _context;

        public ProductionController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.PRO_ProductionOrderHeader
              .Include(x => x.Product)
              .Include(x => x.Recipe)
              .Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive == true)
              .OrderByDescending(x => x.ProductionOrderID)
              .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ProductionOrderViewModel
            {
                Header = new PRO_ProductionOrderHeader
                {
                    CompanyID = CurrentCompanyID,
                    PlannedDate = DateTime.Now
                },
                Details = new List<PRO_ProductionOrderDetail>(),
                FGItems = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.ItemType == "FG" && x.Status && x.IsActive).ToListAsync(),
                Recipes = await _context.PRO_RecipeHeader.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync(),
                Materials = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync(),
                UOMs = await _context.INV_SET_UOM.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductionOrderViewModel vm)
        {
            try
            {
                var maxId = await _context.PRO_ProductionOrderHeader
                  .Where(x => x.CompanyID == CurrentCompanyID)
                  .MaxAsync(x => (int?)x.ProductionOrderID)?? 0;

                vm.Header.CompanyID = CurrentCompanyID;
                vm.Header.ProductionOrderID = maxId + 1;
                vm.Header.ProductionOrderNo = $"PORD-{vm.Header.ProductionOrderID:00000}";
                vm.Header.StatusCode = (short)TransactionStatus.InProcess;
                vm.Header.Status = true;
                vm.Header.CreateDate = DateTime.Now;
                vm.Header.CreatedBy = CurrentUserID;
                vm.Header.CreatedByValue = CurrentUserName;

                _context.PRO_ProductionOrderHeader.Add(vm.Header);
                await _context.SaveChangesAsync();

                int detailId = 1;
                foreach (var d in vm.Details)
                {
                    d.CompanyID = CurrentCompanyID;
                    d.ProductionOrderID = vm.Header.ProductionOrderID;
                    d.ProductionOrderDetailID = detailId++;
                    d.Status = true;
                    d.CreateDate = DateTime.Now;
                    d.CreatedBy = CurrentUserID;
                    d.CreatedByValue = CurrentUserName;
                    _context.PRO_ProductionOrderDetail.Add(d);
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Production Order {vm.Header.ProductionOrderNo} Created";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message?? ex.Message;
                vm.FGItems = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID).ToListAsync();
                vm.Recipes = await _context.PRO_RecipeHeader.Where(x => x.CompanyID == CurrentCompanyID).ToListAsync();
                vm.Materials = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID).ToListAsync();
                vm.UOMs = await _context.INV_SET_UOM.Where(x => x.CompanyID == CurrentCompanyID).ToListAsync();
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var header = await _context.PRO_ProductionOrderHeader
               .Include(x => x.OrderDetails)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionOrderID == id);

            if (header == null) return NotFound();

            // Approved lock
            if (header.StatusCode == (short)TransactionStatus.Approved)
            {
                TempData["Error"] = "Approved document cannot be edited";
                return RedirectToAction("Details", new { id });
            }

            var vm = new ProductionOrderViewModel
            {
                Header = header,
                Details = header.OrderDetails.ToList(),
                FGItems = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync(),
                Recipes = await _context.PRO_RecipeHeader.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync(),
                Materials = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync(),
                UOMs = await _context.INV_SET_UOM.Where(x => x.CompanyID == CurrentCompanyID && x.Status && x.IsActive).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductionOrderViewModel vm)
        {
            var existing = await _context.PRO_ProductionOrderHeader
               .Include(x => x.OrderDetails)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionOrderID == vm.Header.ProductionOrderID);

            if (existing == null) return NotFound();

            if (existing.StatusCode == (short)TransactionStatus.Approved)
                return BadRequest("Approved document");

            existing.ProductID = vm.Header.ProductID;
            existing.RecipeID = vm.Header.RecipeID;
            existing.PlannedQty = vm.Header.PlannedQty;
            existing.BatchSize = vm.Header.BatchSize;
            existing.PlannedDate = vm.Header.PlannedDate;
            existing.Remarks = vm.Header.Remarks;
            existing.UpdateDate = DateTime.Now;
            existing.UpdatedBy = CurrentUserID;
            existing.UpdatedByValue = CurrentUserName;

            // Replace details
            _context.PRO_ProductionOrderDetail.RemoveRange(existing.OrderDetails);

            int detailId = 1;
            foreach (var d in vm.Details)
            {
                d.CompanyID = CurrentCompanyID;
                d.ProductionOrderID = existing.ProductionOrderID;
                d.ProductionOrderDetailID = detailId++;
                d.Status = true;
                d.CreateDate = DateTime.Now;
                d.CreatedBy = CurrentUserID;
                d.CreatedByValue = CurrentUserName;
            }
            await _context.PRO_ProductionOrderDetail.AddRangeAsync(vm.Details);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Updated";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var header = await _context.PRO_ProductionOrderHeader
              .Include(x => x.Product)
              .Include(x => x.Recipe)
              .Include(x => x.OrderDetails).ThenInclude(x => x.Material)
              .Include(x => x.OrderDetails).ThenInclude(x => x.UOM)
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionOrderID == id);
            if (header == null) return NotFound();
            return View(header);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var doc = await _context.PRO_ProductionOrderHeader.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionOrderID == id);
            if (doc == null) return NotFound();

            doc.StatusCode = (short)TransactionStatus.Approved;
            doc.UpdateDate = DateTime.Now;
            doc.UpdatedBy = CurrentUserID;
            doc.UpdatedByValue = CurrentUserName;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Approved - Now locked";
            return RedirectToAction("Details", new { id });
        }

        // For Recipe auto-fetch via AJAX
        [HttpGet]
        public async Task<IActionResult> GetRecipeDetails(int recipeId)
        {
            var details = await _context.PRO_RecipeDetail
            .Where(x => x.CompanyID == CurrentCompanyID && x.RecipeID == recipeId)
            .Select(x => new {
                    materialID = x.MaterialID,
                    quantityRequired = x.QuantityRequired,
                    uomId = x.UOMId,
                    wastagePercent = x.WastagePercent
                }).ToListAsync();
            return Json(details);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipeHeader(int recipeId)
        {
            var header = await _context.PRO_RecipeHeader
            .Where(x => x.CompanyID == CurrentCompanyID && x.RecipeID == recipeId)
            .Select(x => new {
                batchSize = x.BatchSize,
                batchUOM = x.BatchSizeUOM
            }).FirstOrDefaultAsync();
            return Json(header);
        }
    }
}