using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Production;
using KerashineERP.Models.ViewModels;

namespace KerashineERP.Controllers.Production
{
    [Authorize]
    public class ProductionController : BaseController
    {
        private readonly AppDbContext _context;

        public ProductionController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status = "ALL")
        {
            var query = _context.PRO_ProductionHeader
             .Include(x => x.Product)
             .Where(x => x.CompanyID == CurrentCompanyID && x.Status);

            if (status!= "ALL" && short.TryParse(status, out short statusCode))
                query = query.Where(x => x.StatusCode == statusCode);

            var list = await query
             .OrderByDescending(x => x.ProductionDate)
             .ThenByDescending(x => x.BatchNo)
             .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(list);
        }

        public async Task<IActionResult> Details(long id)
        {
            var header = await _context.PRO_ProductionHeader
             .Include(x => x.Product)
             .Include(x => x.ProductionDetails!) // ✅ Fix 1: null-forgiving operator
                 .ThenInclude(d => d.Material!) // ✅ Fix 2: null-forgiving operator
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionID == id);

            if (header == null) return NotFound();
            return View(header);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ProductionViewModel
            {
                Header = new PRO_ProductionHeader
                {
                    ProductionDate = DateTime.Now,
                    CompanyID = CurrentCompanyID,
                    FiscalID = CurrentFiscalID,
                    StatusCode = 1
                },
                Details = new List<PRO_ProductionDetail> { new() }
            };

            var lastBatch = await _context.PRO_ProductionHeader
            .Where(x => x.CompanyID == CurrentCompanyID && x.FiscalID == CurrentFiscalID)
            .OrderByDescending(x => x.ProductionID)
            .Select(x => x.BatchNo)
            .FirstOrDefaultAsync();
            
            int batchNum = 0;
            if (!string.IsNullOrEmpty(lastBatch) && lastBatch.StartsWith("B"))
            {
                int.TryParse(lastBatch.Substring(1), out batchNum);
            }

            model.Header.BatchNo = $"B{(batchNum + 1):D3}";

            return View(await ReloadViewModel(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductionViewModel model)
        {
            model.Details??= new List<PRO_ProductionDetail>();
            model.Details.RemoveAll(x => x.MaterialID == 0);

            if (model.Details.Count == 0)
            {
                TempData["Error"] = "At least one material is required";
                return View(await ReloadViewModel(model));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Validation Failed";
                return View(await ReloadViewModel(model));
            }

            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                var lastBatch = await _context.PRO_ProductionHeader
                 .Where(x => x.CompanyID == CurrentCompanyID && x.FiscalID == CurrentFiscalID)
                 .OrderByDescending(x => x.ProductionID)
                 .Select(x => x.BatchNo)
                 .FirstOrDefaultAsync();

                int batchNum = 0;
                if (!string.IsNullOrEmpty(lastBatch) && lastBatch.StartsWith("B"))
                {
                    int.TryParse(lastBatch.Substring(1), out batchNum);
                }
                
                model.Header.BatchNo = $"B{(batchNum + 1):D3}";
                model.Header.CompanyID = CurrentCompanyID;
                model.Header.FiscalID = CurrentFiscalID;
                model.Header.TotalCost = model.Details.Sum(x => x.QuantityUsed * x.CostPerUnit);
                model.Header.CostPerUnit = model.Header.QuantityProduced > 0 
                 ? model.Header.TotalCost / model.Header.QuantityProduced : 0;
                model.Header.Status = true;
                model.Header.CreateDate = DateTime.Now;
                model.Header.CreatedBy = CurrentUserID;
                model.Header.CreatedByValue = CurrentUserName;
                
                _context.PRO_ProductionHeader.Add(model.Header);
                await _context.SaveChangesAsync();

                long detailId = 1;
                foreach (var d in model.Details)
                {
                    d.ProductionID = model.Header.ProductionID;
                    d.CompanyID = CurrentCompanyID;
                    d.ProductionDetailID = detailId++;
                    d.TotalCost = d.QuantityUsed * d.CostPerUnit;
                    d.Status = true;
                    d.CreateDate = DateTime.Now;
                    d.CreatedBy = CurrentUserID;
                    d.CreatedByValue = CurrentUserName;
                }

                _context.PRO_ProductionDetail.AddRange(model.Details);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();

                TempData["Success"] = $"Batch {model.Header.BatchNo} saved successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                TempData["Error"] = "Error: " + ex.Message;
                return View(await ReloadViewModel(model));
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            var header = await _context.PRO_ProductionHeader
             .Include(x => x.ProductionDetails) // ✅ Collection load karo
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionID == id);

            if (header == null) return NotFound();

            if (header.StatusCode!= 1)
            {
                TempData["Error"] = "Only In-Process batches can be edited";
                return RedirectToAction("Index");
            }

            var model = new ProductionViewModel
            {
                Header = header,
                Details = header.ProductionDetails?.ToList()?? new List<PRO_ProductionDetail>() // ✅ Fix 3: Null check
            };

            return View(await ReloadViewModel(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ProductionViewModel model)
        {
            var existing = await _context.PRO_ProductionHeader
             .Include(x => x.ProductionDetails) // ✅ Collection load karo
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionID == id);

            if (existing == null) return NotFound();

            if (existing.StatusCode!= 1)
            {
                TempData["Error"] = "Only In-Process batches can be edited";
                return RedirectToAction("Index");
            }

            model.Details??= new List<PRO_ProductionDetail>();
            model.Details.RemoveAll(x => x.MaterialID == 0);

            if (model.Details.Count == 0)
            {
                TempData["Error"] = "At least one material is required";
                return View(await ReloadViewModel(model));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Validation Failed";
                return View(await ReloadViewModel(model));
            }

            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ Fix 4: Null check before RemoveRange
                if (existing.ProductionDetails!= null && existing.ProductionDetails.Any())
                {
                    _context.PRO_ProductionDetail.RemoveRange(existing.ProductionDetails);
                    await _context.SaveChangesAsync();
                }

                existing.ProductionDate = model.Header.ProductionDate;
                existing.ProductID = model.Header.ProductID;
                existing.QuantityProduced = model.Header.QuantityProduced;
                existing.Remarks = model.Header.Remarks;
                existing.UpdateDate = DateTime.Now;
                existing.UpdatedBy = CurrentUserID;
                existing.UpdatedByValue = CurrentUserName;

                long detailId = 1;
                var detailsToSave = new List<PRO_ProductionDetail>();
                foreach (var d in model.Details)
                {
                    d.CompanyID = CurrentCompanyID;
                    d.ProductionID = id;
                    d.ProductionDetailID = detailId++;
                    d.TotalCost = d.QuantityUsed * d.CostPerUnit;
                    d.Status = true;
                    d.CreateDate = DateTime.Now;
                    d.CreatedBy = CurrentUserID;
                    d.CreatedByValue = CurrentUserName;
                    detailsToSave.Add(d);
                }

                existing.TotalCost = detailsToSave.Sum(x => x.TotalCost);
                existing.CostPerUnit = existing.QuantityProduced > 0 
                 ? existing.TotalCost / existing.QuantityProduced : 0;

                _context.PRO_ProductionDetail.AddRange(detailsToSave);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();

                TempData["Success"] = "Production updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                TempData["Error"] = "Error: " + ex.Message;
                return View(await ReloadViewModel(model));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var header = await _context.PRO_ProductionHeader
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionID == id);

            if (header == null) return NotFound();

            if (header.StatusCode!= 1)
            {
                TempData["Error"] = "Only In-Process batches can be deleted";
                return RedirectToAction("Index");
            }

            header.Status = false;
            header.UpdateDate = DateTime.Now;
            header.UpdatedBy = CurrentUserID;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Production deleted successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(long id)
        {
            var header = await _context.PRO_ProductionHeader
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ProductionID == id);

            if (header == null) return NotFound();

            header.StatusCode = 2;
            header.UpdateDate = DateTime.Now;
            header.UpdatedBy = CurrentUserID;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Production completed successfully";
            return RedirectToAction("Index");
        }

        private async Task<ProductionViewModel> ReloadViewModel(ProductionViewModel model)
        {
            model.Products = await _context.INV_Item
             .Where(x => x.CompanyID == CurrentCompanyID && x.IsActive && x.Status && x.ItemType == "FG")
             .OrderBy(x => x.ItemCode)
             .ToListAsync();

            model.RawMaterials = await _context.INV_Item
             .Where(x => x.CompanyID == CurrentCompanyID && x.IsActive && x.Status && (x.ItemType == "RM"))
             .OrderBy(x => x.ItemType).ThenBy(x => x.ItemCode)
             .ToListAsync();

            if (model.Details == null || model.Details.Count == 0)
                model.Details = new List<PRO_ProductionDetail> { new() };

            return model;
        }
    }
}