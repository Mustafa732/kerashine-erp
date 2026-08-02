using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Purchase;
using KerashineERP.Models.ViewModels;

namespace KerashineERP.Controllers.Purchase
{
    [Authorize]
    public class PurchaseOrderController : BaseController
    {
        private readonly AppDbContext _context;
        public PurchaseOrderController(AppDbContext context) => _context = context;

        // ===== INDEX =====
        public async Task<IActionResult> Index(string status = "ALL")
        {
            var query = _context.AP_PurchaseOrderHeader
               .Include(x => x.Vendor)
               .Where(x => x.CompanyID == CurrentCompanyID && x.Status == true);

            if (status != "ALL" && short.TryParse(status, out short sc))
                query = query.Where(x => x.StatusCode == sc);

            var list = await query.OrderByDescending(x => x.PODate).ThenByDescending(x => x.DocumentNumber).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(list);
        }

        // ===== DETAILS =====
        public async Task<IActionResult> Details(int id)
        {
            var header = await _context.AP_PurchaseOrderHeader
               .Include(x => x.Vendor)
               .Include(x => x.Details)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == id);

            if (header == null) return NotFound();
            return View(header);
        }

        // ===== CREATE =====
        public async Task<IActionResult> Create(int docTypeId = 0)
        {
            var vm = new PurchaseOrderVM
            {
                Header = new AP_PurchaseOrderHeader
                {
                    PODate = DateTime.Now,
                    CompanyID = CurrentCompanyID,
                    DocumentTypeID = docTypeId,
                    StatusCode = (short)POStatusCode.InProcess,
                    ClosedCode = (short)POClosedCode.Open
                },
                Details = new List<AP_PurchaseOrderDetail> { new() }
            };

            if (docTypeId > 0)
            {
                var maxDocNo = await _context.AP_PurchaseOrderHeader
                   .Where(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == docTypeId)
                   .MaxAsync(x => (int?)x.DocumentNumber) ?? 0;
                vm.Header.DocumentNumber = maxDocNo + 1;
                vm.SelectedDocTypeID = docTypeId;
            }

            vm = await ReloadVM(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderVM vm)
        {
            vm.Details ??= new List<AP_PurchaseOrderDetail>();
            vm.Details.RemoveAll(x => x.ItemID == 0 || x.Quantity <= 0);

            if (vm.Details.Count == 0)
            {
                ModelState.AddModelError("", "At least one item is required");
                return View(await ReloadVM(vm));
            }

            if (!ModelState.IsValid)
                return View(await ReloadVM(vm));

            using var tran = await _context.Database.BeginTransactionAsync();
            try
            {
                var maxHeaderId = await _context.AP_PurchaseOrderHeader
                   .Where(x => x.CompanyID == CurrentCompanyID)
                   .MaxAsync(x => (int?)x.POHeaderID) ?? 0;

                var maxDocNo = await _context.AP_PurchaseOrderHeader
                   .Where(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == vm.Header.DocumentTypeID)
                   .MaxAsync(x => (int?)x.DocumentNumber) ?? 0;

                vm.Header.CompanyID = CurrentCompanyID;
                vm.Header.POHeaderID = maxHeaderId + 1;
                vm.Header.DocumentNumber = maxDocNo + 1;
                vm.Header.CreatedBy = CurrentUserID;
                vm.Header.CreatedByValue = CurrentUserName;
                vm.Header.CreatedDate = DateTime.Now;
                vm.Header.Status = true;
                vm.Header.StatusCode = (short)POStatusCode.InProcess;
                vm.Header.ClosedCode = (short)POClosedCode.Open;

                // Calc totals
                foreach (var d in vm.Details)
                    d.Amount = d.Quantity * d.Rate;

                vm.Header.GrossAmount = vm.Details.Sum(x => x.Amount);
                vm.Header.NetAmount = vm.Header.GrossAmount - vm.Header.DiscountAmount;

                _context.AP_PurchaseOrderHeader.Add(vm.Header);
                await _context.SaveChangesAsync();

                int detailId = 1;
                int serial = 1;
                var detailsToSave = new List<AP_PurchaseOrderDetail>();
                foreach (var d in vm.Details)
                {
                    d.CompanyID = CurrentCompanyID;
                    d.POHeaderID = vm.Header.POHeaderID;
                    d.PODetailID = detailId++;
                    d.SerialNo = serial++;
                    d.Status = true;
                    d.CreatedBy = CurrentUserID;
                    d.CreatedByValue = CurrentUserName;
                    d.CreatedDate = DateTime.Now;
                    detailsToSave.Add(d);
                }

                _context.AP_PurchaseOrderDetail.AddRange(detailsToSave);
                await _context.SaveChangesAsync();
                await tran.CommitAsync();

                TempData["Success"] = $"PO {vm.Header.DocumentNumber} saved successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                ModelState.AddModelError("", "Error: " + ex.InnerException?.Message ?? ex.Message);
                return View(await ReloadVM(vm));
            }
        }

        // ===== EDIT =====
        public async Task<IActionResult> Edit(int id)
        {
            var header = await _context.AP_PurchaseOrderHeader
               .Include(x => x.Details)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == id);

            if (header == null) return NotFound();
            if (header.StatusCode != (short)POStatusCode.InProcess)
            {
                TempData["Error"] = "Only In-Process PO can be edited";
                return RedirectToAction("Index");
            }

            var vm = new PurchaseOrderVM
            {
                Header = header,
                Details = header.Details.OrderBy(x => x.SerialNo).ToList(),
                SelectedDocTypeID = header.DocumentTypeID
            };
            return View(await ReloadVM(vm));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseOrderVM vm)
        {
            var existing = await _context.AP_PurchaseOrderHeader
               .Include(x => x.Details)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == id);

            if (existing == null) return NotFound();
            if (existing.StatusCode != (short)POStatusCode.InProcess)
            {
                TempData["Error"] = "Only In-Process PO can be edited";
                return RedirectToAction("Index");
            }

            vm.Details ??= new List<AP_PurchaseOrderDetail>();
            vm.Details.RemoveAll(x => x.ItemID == 0 || x.Quantity <= 0);

            if (vm.Details.Count == 0)
            {
                ModelState.AddModelError("", "At least one item is required");
                return View(await ReloadVM(vm));
            }

            using var tran = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.AP_PurchaseOrderDetail.RemoveRange(existing.Details);
                await _context.SaveChangesAsync();

                existing.PODate = vm.Header.PODate;
                existing.VendorID = vm.Header.VendorID;
                existing.Remarks = vm.Header.Remarks;
                existing.DiscountAmount = vm.Header.DiscountAmount;
                existing.UpdatedBy = CurrentUserID.GetHashCode(); // tumhara UpdatedBy int hai
                existing.UpdatedByValue = CurrentUserName;
                existing.UpdatedDate = DateTime.Now;

                foreach (var d in vm.Details)
                    d.Amount = d.Quantity * d.Rate;

                existing.GrossAmount = vm.Details.Sum(x => x.Amount);
                existing.NetAmount = existing.GrossAmount - existing.DiscountAmount;

                int detailId = 1;
                int serial = 1;
                var detailsToSave = new List<AP_PurchaseOrderDetail>();
                foreach (var d in vm.Details)
                {
                    d.CompanyID = CurrentCompanyID;
                    d.POHeaderID = id;
                    d.PODetailID = detailId++;
                    d.SerialNo = serial++;
                    d.Status = true;
                    d.CreatedBy = existing.CreatedBy;
                    d.CreatedByValue = existing.CreatedByValue;
                    d.CreatedDate = existing.CreatedDate;
                    detailsToSave.Add(d);
                }

                _context.AP_PurchaseOrderDetail.AddRange(detailsToSave);
                await _context.SaveChangesAsync();
                await tran.CommitAsync();

                TempData["Success"] = "PO updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(await ReloadVM(vm));
            }
        }

        // ===== DELETE (Soft) =====
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var header = await _context.AP_PurchaseOrderHeader
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == id);

            if (header == null) return NotFound();
            if (header.StatusCode != (short)POStatusCode.InProcess)
            {
                TempData["Error"] = "Only In-Process PO can be deleted";
                return RedirectToAction("Index");
            }

            header.Status = false;
            header.UpdatedDate = DateTime.Now;
            header.UpdatedByValue = CurrentUserName;
            await _context.SaveChangesAsync();

            TempData["Success"] = "PO deleted successfully";
            return RedirectToAction("Index");
        }

        // ===== APPROVE =====
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var header = await _context.AP_PurchaseOrderHeader
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == id);

            if (header == null) return NotFound();

            header.StatusCode = (short)POStatusCode.Approved;
            header.UpdatedDate = DateTime.Now;
            header.UpdatedByValue = CurrentUserName;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"PO {header.DocumentNumber} approved successfully";
            return RedirectToAction("Index");
        }

        // ===== Helpers =====
        private async Task<PurchaseOrderVM> ReloadVM(PurchaseOrderVM vm)
        {
            vm.DocumentTypes = await _context.SET_DocumentType
               .Where(x => x.CompanyID == CurrentCompanyID && x.IsActive)
               .ToListAsync();

            // Vendor = Customer jiska Type = 1 (Vendor)
            var vendorTypeCustomerIds = await _context.AR_SET_BusinessPartnerTypes
               .Where(x => x.CompanyID == CurrentCompanyID && x.TypeCode == 1)
               .Select(x => x.BusinessPartnerID).ToListAsync();

            vm.Vendors = await _context.AR_SET_Customer
               .Where(x => x.CompanyID == CurrentCompanyID && vendorTypeCustomerIds.Contains(x.CustomerID) && x.IsActive)
               .ToListAsync();

            vm.Items = await _context.INV_Item
               .Where(x => x.CompanyID == CurrentCompanyID && x.IsActive && x.ItemType == "RM")
               .OrderBy(x => x.ItemName).ToListAsync();

            if (vm.Details == null || vm.Details.Count == 0)
                vm.Details = new List<AP_PurchaseOrderDetail> { new() };

            return vm;
        }

        public async Task<JsonResult> GetAvgRate(int itemId)
        {
            // Weighted Average: Total Amount / Total Qty (only IN transactions)
            var stockData = await (
                from d in _context.INV_StockTransactionDetail
                join h in _context.INV_StockTransactionHeader
                    on new { d.CompanyID, d.StockTransactionID } equals new { h.CompanyID, h.StockTransactionID }
                where d.CompanyID == CurrentCompanyID
                   && d.ItemID == itemId
                   && h.Status == true
                   && d.Quantity > 0 // Only IN
                select new { d.Quantity, d.Amount, d.Rate }
            ).ToListAsync();

            decimal avgRate = 0;
            if (stockData.Any())
            {
                var totalQty = stockData.Sum(x => x.Quantity);
                var totalAmt = stockData.Sum(x => x.Amount);
                if (totalQty > 0)
                    avgRate = totalAmt / totalQty;
            }

            // Agar pehli dafa item hai to 0 ayega, to last purchase rate se lelo
            if (avgRate == 0)
            {
                var lastRate = await _context.AP_PurchaseOrderDetail
                   .Where(x => x.CompanyID == CurrentCompanyID && x.ItemID == itemId)
                   .OrderByDescending(x => x.CreatedDate)
                   .Select(x => x.Rate)
                   .FirstOrDefaultAsync();
                avgRate = lastRate;
            }

            var uom = await _context.INV_Item
               .Where(x => x.CompanyID == CurrentCompanyID && x.ItemId == itemId && x.ItemType == "RM")
               .Select(x => x.UOMId)
               .FirstOrDefaultAsync();

            return Json(new
            {
                avgRate = Math.Round(avgRate, 3),
                uomId = uom,
                lastQty = stockData.LastOrDefault()?.Quantity ?? 0
            });
        }
    }
}