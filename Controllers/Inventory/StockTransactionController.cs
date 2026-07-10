using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.ViewModels;
using KerashineERP.Models.Common;

namespace KerashineERP.Controllers.Inventory
{
    [Authorize]
    public class StockTransactionController : BaseController
    {
        private readonly AppDbContext _context;

        public StockTransactionController(AppDbContext context)
        {
            _context = context;
        }

        // ===== INDEX / LIST =====
        public async Task<IActionResult> Index(string docType = "ALL", string status = "ALL")
        {
            var query = _context.INV_StockTransactionHeader
              .Include(x => x.FK_INV_StockTransactionHeader_SET_DocumentType)
              .Include(x => x.FK_INV_StockTransactionHeader_SET_Location_From)
              .Include(x => x.FK_INV_StockTransactionHeader_SET_Location_To)
              .Where(x => x.CompanyID == CurrentCompanyID && x.Status == true);

            if (docType!= "ALL")
                query = query.Where(x => x.FK_INV_StockTransactionHeader_SET_DocumentType.DocumentType == docType);

            if (status!= "ALL" && short.TryParse(status, out short statusCode))
                query = query.Where(x => x.StatusCode == statusCode);

            var transactions = await query
              .OrderByDescending(x => x.DocumentDate)
              .ThenByDescending(x => x.DocumentNo)
              .ToListAsync();

            ViewBag.DocTypes = await _context.SET_DocumentType
              .Where(x => x.CompanyID == CurrentCompanyID && x.IsActive)
              .ToListAsync();
            ViewBag.CurrentDocType = docType;
            ViewBag.CurrentStatus = status;

            return View(transactions);
        }

        // ===== DETAILS / FIND =====
        public async Task<IActionResult> Details(long id)
        {
            var header = await _context.INV_StockTransactionHeader
              .Include(x => x.FK_INV_StockTransactionHeader_SET_DocumentType)
              .Include(x => x.FK_INV_StockTransactionHeader_SET_Location_From)
              .Include(x => x.FK_INV_StockTransactionHeader_SET_Location_To)
              .Include(x => x.StockTransactionDetail)
                  .ThenInclude(d => d.Item)
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);

            if (header == null) return NotFound();

            return View(header);
        }

        // ===== CREATE =====
        public async Task<IActionResult> Create(int docTypeId = 0)
        {
            var model = new StockTransactionViewModel
            {
                Header = new INV_StockTransactionHeader
                {
                    DocumentDate = DateTime.Now,
                    DocumentTypeID = docTypeId,
                    CompanyID = CurrentCompanyID,
                    FiscalID = CurrentFiscalID,
                    StatusCode = (short)TransactionStatus.InProcess
                },
                Details = new List<INV_StockTransactionDetail> { new() }
            };

            // ===== Auto Generate Next DocumentNo for Display =====
            if (docTypeId > 0)
            {
                var maxDocNo = await _context.INV_StockTransactionHeader
                .Where(x => x.CompanyID == CurrentCompanyID
                            && x.FiscalID == CurrentFiscalID
                            && x.DocumentTypeID == docTypeId)
                .MaxAsync(x => (int?)x.DocumentNo)?? 0;

                model.Header.DocumentNo = maxDocNo + 1;
                model.SelectedDocTypeID = docTypeId;
            }

            model = await ReloadViewModel(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionViewModel model)
        {
            model.Details??= new List<INV_StockTransactionDetail>();
            model.Details.RemoveAll(x => x.ItemID == 0);

            if (model.Details.Count == 0)
            {
                ModelState.AddModelError("", "At least one item is required");
                return View(await ReloadViewModel(model));
            }

            var docType = await _context.SET_DocumentType
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == model.Header.DocumentTypeID);

            if (docType == null)
            {
                ModelState.AddModelError("", "Invalid Document Type");
                return View(await ReloadViewModel(model));
            }

            // Location Validation
            if (docType.DocumentType == "TRANSFER")
            {
                if (!model.Header.FromLocationID.HasValue ||!model.Header.ToLocationID.HasValue)
                {
                    ModelState.AddModelError("", "Both From and To Location are required for Transfer");
                    return View(await ReloadViewModel(model));
                }
                if (model.Header.FromLocationID == model.Header.ToLocationID)
                {
                    ModelState.AddModelError("", "From and To Location cannot be same");
                    return View(await ReloadViewModel(model));
                }
            }
            else if (docType.TransactionType == "Debit" &&!model.Header.ToLocationID.HasValue)
            {
                ModelState.AddModelError("", "Location is required for stock IN transaction");
                return View(await ReloadViewModel(model));
            }
            else if (docType.TransactionType == "Credit" &&!model.Header.FromLocationID.HasValue &&!model.Header.ToLocationID.HasValue)
            {
                ModelState.AddModelError("", "Location is required for stock OUT transaction");
                return View(await ReloadViewModel(model));
            }

            // Apply Qty Sign
            if (docType.DocumentType!= "TRANSFER")
            {
                foreach (var detail in model.Details)
                {
                    if (docType.TransactionType == "Credit")
                        detail.Quantity = -Math.Abs(detail.Quantity);
                    else if (docType.TransactionType == "Debit")
                        detail.Quantity = Math.Abs(detail.Quantity);
                    detail.Amount = detail.Quantity * detail.Rate;
                }
            }
            else
            {
                foreach (var detail in model.Details)
                {
                    detail.Quantity = Math.Abs(detail.Quantity);
                    detail.Amount = detail.Quantity * detail.Rate;
                }
            }

            // Stock Validation
            int? stockCheckLocationId = null;
            if (docType.TransactionType == "Credit" || docType.DocumentType == "TRANSFER")
                stockCheckLocationId = model.Header.FromLocationID?? model.Header.ToLocationID;

            if (stockCheckLocationId.HasValue)
            {
                foreach (var detail in model.Details)
                {
                    var qtyToCheck = Math.Abs(detail.Quantity);
                    var currentStock = await GetLocationStock(CurrentCompanyID, detail.ItemID, stockCheckLocationId.Value);

                    if (currentStock < qtyToCheck)
                    {
                        var item = await _context.INV_Item.FindAsync(CurrentCompanyID, detail.ItemID);
                        var location = await _context.SET_Location.FindAsync(CurrentCompanyID, stockCheckLocationId.Value);
                        ModelState.AddModelError("", $"Insufficient stock for {item?.ItemName} at {location?.LocationName}. Available: {currentStock}, Required: {qtyToCheck}");
                    }
                }
            }

            if (!ModelState.IsValid)
                return View(await ReloadViewModel(model));

            // Save
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var maxDocNo = await _context.INV_StockTransactionHeader
                .Where(x => x.CompanyID == CurrentCompanyID && x.FiscalID == CurrentFiscalID && x.DocumentTypeID == model.Header.DocumentTypeID)
                .MaxAsync(x => (int?)x.DocumentNo)?? 0;

                var maxStockTransactionID = await _context.INV_StockTransactionHeader
                .Where(x => x.CompanyID == CurrentCompanyID)
                .MaxAsync(x => (long?)x.StockTransactionID)?? 0;

                model.Header.StockTransactionID = maxStockTransactionID + 1;
                model.Header.DocumentNo = maxDocNo + 1;
                model.Header.CompanyID = CurrentCompanyID;
                model.Header.FiscalID = CurrentFiscalID;
                model.Header.CreateDate = DateTime.Now;
                model.Header.CreatedBy = CurrentUserID;
                model.Header.CreatedByValue = CurrentUserName;
                model.Header.Status = true;
                model.Header.StatusCode = (short)TransactionStatus.InProcess;

                var detailsToSave = new List<INV_StockTransactionDetail>();
                long detailCounter = 1;

                if (docType.DocumentType == "TRANSFER")
                {
                    foreach (var detail in model.Details.Where(x => x.Quantity > 0))
                    {
                        var qty = detail.Quantity;
                        var fromLoc = await _context.SET_Location.FindAsync(CurrentCompanyID, model.Header.FromLocationID);
                        var toLoc = await _context.SET_Location.FindAsync(CurrentCompanyID, model.Header.ToLocationID);

                        detailsToSave.Add(new INV_StockTransactionDetail
                        {
                            CompanyID = CurrentCompanyID,
                            TransactionDetailID = detailCounter++,
                            ItemID = detail.ItemID,
                            BatchNo = detail.BatchNo,
                            ExpiryDate = detail.ExpiryDate,
                            Quantity = -qty,
                            Rate = detail.Rate,
                            Amount = -qty * detail.Rate,
                            Remarks = $"Transfer to {toLoc?.LocationName} - {detail.Remarks}".Trim(),
                            Status = true,
                            CreateDate = DateTime.Now,
                            CreatedBy = CurrentUserID,
                            CreatedByValue = CurrentUserName
                        });

                        detailsToSave.Add(new INV_StockTransactionDetail
                        {
                            CompanyID = CurrentCompanyID,
                            TransactionDetailID = detailCounter++,
                            ItemID = detail.ItemID,
                            BatchNo = detail.BatchNo,
                            ExpiryDate = detail.ExpiryDate,
                            Quantity = qty,
                            Rate = detail.Rate,
                            Amount = qty * detail.Rate,
                            Remarks = $"Transfer from {fromLoc?.LocationName} - {detail.Remarks}".Trim(),
                            Status = true,
                            CreateDate = DateTime.Now,
                            CreatedBy = CurrentUserID,
                            CreatedByValue = CurrentUserName
                        });
                    }
                }
                else
                {
                    foreach (var detail in model.Details)
                    {
                        detail.CompanyID = CurrentCompanyID;
                        detail.TransactionDetailID = detailCounter++;
                        detail.Status = true;
                        detail.CreateDate = DateTime.Now;
                        detail.CreatedBy = CurrentUserID;
                        detail.CreatedByValue = CurrentUserName;
                        detailsToSave.Add(detail);
                    }
                }

                model.Header.TotalAmount = detailsToSave.Sum(x => x.Amount);
                _context.INV_StockTransactionHeader.Add(model.Header);
                await _context.SaveChangesAsync();

                foreach (var detail in detailsToSave)
                    detail.StockTransactionID = model.Header.StockTransactionID;

                _context.INV_StockTransactionDetail.AddRange(detailsToSave);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Document {model.Header.DocumentNo} saved successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(await ReloadViewModel(model));
            }
        }

        // ===== EDIT =====
        public async Task<IActionResult> Edit(long id)
        {
            var header = await _context.INV_StockTransactionHeader
              .Include(x => x.StockTransactionDetail)
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);

            if (header == null) return NotFound();

            if (header.StatusCode!= (short)TransactionStatus.InProcess)
            {
                TempData["Error"] = "Only In-Process transactions can be edited";
                return RedirectToAction("Index");
            }

            var docType = await _context.SET_DocumentType.FindAsync(CurrentCompanyID, header.DocumentTypeID);
            
            // TRANSFER mein sirf positive rows show karo edit ke time
            var details = docType?.DocumentType == "TRANSFER" 
               ? header.StockTransactionDetail.Where(x => x.Quantity > 0).ToList()
                : header.StockTransactionDetail.ToList();

            var model = new StockTransactionViewModel
            {
                Header = header,
                Details = details,
                SelectedDocTypeID = header.DocumentTypeID
            };

            return View(await ReloadViewModel(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, StockTransactionViewModel model)
        {
            var existing = await _context.INV_StockTransactionHeader
              .Include(x => x.StockTransactionDetail)
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);

            if (existing == null) return NotFound();

            if (existing.StatusCode!= (short)TransactionStatus.InProcess)
            {
                TempData["Error"] = "Only In-Process transactions can be edited";
                return RedirectToAction("Index");
            }

            model.Details??= new List<INV_StockTransactionDetail>();
            model.Details.RemoveAll(x => x.ItemID == 0);

            if (model.Details.Count == 0)
            {
                ModelState.AddModelError("", "At least one item is required");
                return View(await ReloadViewModel(model));
            }

            var docType = await _context.SET_DocumentType.FindAsync(CurrentCompanyID, model.Header.DocumentTypeID);

            if (docType == null)
            {
                ModelState.AddModelError("", "Invalid Document Type");
                return View(await ReloadViewModel(model));
            }

            // Same validation as Create
            if (docType.DocumentType == "TRANSFER")
            {
                if (!model.Header.FromLocationID.HasValue ||!model.Header.ToLocationID.HasValue)
                {
                    ModelState.AddModelError("", "Both From and To Location are required for Transfer");
                    return View(await ReloadViewModel(model));
                }
                if (model.Header.FromLocationID == model.Header.ToLocationID)
                {
                    ModelState.AddModelError("", "From and To Location cannot be same");
                    return View(await ReloadViewModel(model));
                }
            }

            // Stock Validation - SAME AS CREATE
            int? stockCheckLocationId = null;
            if (docType.TransactionType == "Credit" || docType.DocumentType == "TRANSFER")
                stockCheckLocationId = model.Header.FromLocationID?? model.Header.ToLocationID;

            if (stockCheckLocationId.HasValue)
            {
                foreach (var detail in model.Details)
                {
                    var qtyToCheck = Math.Abs(detail.Quantity);
                    var currentStock = await GetLocationStock(CurrentCompanyID, detail.ItemID, stockCheckLocationId.Value);

                    if (currentStock < qtyToCheck)
                    {
                        var item = await _context.INV_Item.FindAsync(CurrentCompanyID, detail.ItemID);
                        var location = await _context.SET_Location.FindAsync(CurrentCompanyID, stockCheckLocationId.Value);
                        ModelState.AddModelError("", $"Insufficient stock for {item?.ItemName} at {location?.LocationName}. Available: {currentStock}, Required: {qtyToCheck}");
                    }
                }
            }

            if (!ModelState.IsValid)
                return View(await ReloadViewModel(model));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.INV_StockTransactionDetail.RemoveRange(existing.StockTransactionDetail);
                await _context.SaveChangesAsync();

                existing.DocumentDate = model.Header.DocumentDate;
                existing.FromLocationID = model.Header.FromLocationID;
                existing.ToLocationID = model.Header.ToLocationID;
                existing.Remarks = model.Header.Remarks;
                existing.UpdateDate = DateTime.Now;
                existing.UpdatedBy = CurrentUserID;
                existing.UpdatedByValue = CurrentUserName;

                var detailsToSave = new List<INV_StockTransactionDetail>();
                long detailCounter = 1;

                if (docType.DocumentType == "TRANSFER")
                {
                    var fromLoc = await _context.SET_Location.FindAsync(CurrentCompanyID, model.Header.FromLocationID);
                    var toLoc = await _context.SET_Location.FindAsync(CurrentCompanyID, model.Header.ToLocationID);

                    foreach (var detail in model.Details)
                    {
                        var qty = Math.Abs(detail.Quantity);
                        detailsToSave.Add(new INV_StockTransactionDetail
                        {
                            CompanyID = CurrentCompanyID,
                            StockTransactionID = id,
                            TransactionDetailID = detailCounter++,
                            ItemID = detail.ItemID,
                            BatchNo = detail.BatchNo,
                            ExpiryDate = detail.ExpiryDate,
                            Quantity = -qty,
                            Rate = detail.Rate,
                            Amount = -qty * detail.Rate,
                            Remarks = $"Transfer to {toLoc?.LocationName} - {detail.Remarks}".Trim(),
                            Status = true,
                            CreateDate = DateTime.Now,
                            CreatedBy = CurrentUserID,
                            CreatedByValue = CurrentUserName
                        });
                        detailsToSave.Add(new INV_StockTransactionDetail
                        {
                            CompanyID = CurrentCompanyID,
                            StockTransactionID = id,
                            TransactionDetailID = detailCounter++,
                            ItemID = detail.ItemID,
                            BatchNo = detail.BatchNo,
                            ExpiryDate = detail.ExpiryDate,
                            Quantity = qty,
                            Rate = detail.Rate,
                            Amount = qty * detail.Rate,
                            Remarks = $"Transfer from {fromLoc?.LocationName} - {detail.Remarks}".Trim(),
                            Status = true,
                            CreateDate = DateTime.Now,
                            CreatedBy = CurrentUserID,
                            CreatedByValue = CurrentUserName
                        });
                    }
                }
                else
                {
                    foreach (var detail in model.Details)
                    {
                        if (docType.TransactionType == "Credit")
                            detail.Quantity = -Math.Abs(detail.Quantity);
                        else
                            detail.Quantity = Math.Abs(detail.Quantity);
                        detail.Amount = detail.Quantity * detail.Rate;
                        detail.CompanyID = CurrentCompanyID;
                        detail.StockTransactionID = id;
                        detail.TransactionDetailID = detailCounter++;
                        detail.Status = true;
                        detail.CreateDate = DateTime.Now;
                        detail.CreatedBy = CurrentUserID;
                        detail.CreatedByValue = CurrentUserName;
                        detailsToSave.Add(detail);
                    }
                }

                existing.TotalAmount = detailsToSave.Sum(x => x.Amount);
                _context.INV_StockTransactionDetail.AddRange(detailsToSave);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Transaction updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(await ReloadViewModel(model));
            }
        }

        // ===== DELETE / APPROVE =====
        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var header = await _context.INV_StockTransactionHeader
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);

            if (header == null) return NotFound();

            if (header.StatusCode!= (short)TransactionStatus.InProcess)
            {
                TempData["Error"] = "Only In-Process transactions can be deleted";
                return RedirectToAction("Index");
            }

            header.Status = false;
            header.UpdateDate = DateTime.Now;
            header.UpdatedBy = CurrentUserID;
            header.UpdatedByValue = CurrentUserName;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Transaction deleted successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Approve(long id)
        {
            var header = await _context.INV_StockTransactionHeader
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);

            if (header == null) return NotFound();

            header.StatusCode = (short)TransactionStatus.Approved;
            header.UpdateDate = DateTime.Now;
            header.UpdatedBy = CurrentUserID;
            header.UpdatedByValue = CurrentUserName;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Transaction approved successfully";
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> GetItemDetails(int itemId)
        {
            var item = await _context.INV_Item
            .Where(x => x.ItemId == itemId && x.CompanyID == CurrentCompanyID)
            .Select(x => new {
                   uomId = x.UOMId,
                   displayName = x.ItemName
               })
            .FirstOrDefaultAsync();
            return Json(item);
        }

        public async Task<JsonResult> GetLocationStock(int itemId, int locationId)
        {
            var stock = await GetLocationStock(CurrentCompanyID, itemId, locationId);
            return Json(new { stock = stock });
        }

        private async Task<StockTransactionViewModel> ReloadViewModel(StockTransactionViewModel model)
        {
            model.DocumentTypes = await _context.SET_DocumentType
            .Where(x => x.IsActive && x.CompanyID == CurrentCompanyID)
            .OrderBy(x => x.DocumentTypeID)
            .ToListAsync();

            model.Locations = await _context.SET_Location
            .Where(x => x.IsActive && x.CompanyID == CurrentCompanyID)
            .ToListAsync();

            model.Items = await _context.INV_Item
            .Where(x => x.IsActive && x.CompanyID == CurrentCompanyID)
            .OrderBy(x => x.ItemType).ThenBy(x => x.ItemCode)
            .ToListAsync();

            model.UOMs = await _context.INV_SET_UOM
            .Where(x => x.IsActive && x.CompanyID == CurrentCompanyID)
            .ToListAsync();

            if (model.Details == null || model.Details.Count == 0)
                model.Details = new List<INV_StockTransactionDetail> { new() };

            return model;
        }

        private async Task<decimal> GetLocationStock(int companyId, int itemId, int locationId)
        {
            var stock = await (
                from d in _context.INV_StockTransactionDetail
                join h in _context.INV_StockTransactionHeader
                    on new { d.CompanyID, d.StockTransactionID } equals new { h.CompanyID, h.StockTransactionID }
                where d.CompanyID == companyId
                    && d.ItemID == itemId
                    && h.Status == true
                    && (h.ToLocationID == locationId || h.FromLocationID == locationId)
                select h.ToLocationID == locationId? d.Quantity : -d.Quantity
            ).SumAsync();

            return stock;
        }
    }
}