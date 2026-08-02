using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.ViewModels;
using KerashineERP.Models.Common;
using KerashineERP.Models.Purchase;

namespace KerashineERP.Controllers.Inventory
{
    [Authorize]
    public class GRNController : BaseController
    {
        private readonly AppDbContext _context;
        public GRNController(AppDbContext context) => _context = context;

        // INDEX - Sirf GRN type ke docs
        public async Task<IActionResult> Index(string status = "ALL")
        {
            var grnDocTypeIds = await _context.SET_DocumentType
              .Where(x => x.CompanyID == CurrentCompanyID && x.DocumentType == "GRN")
              .Select(x => x.DocumentTypeID).ToListAsync();

            var query = _context.INV_StockTransactionHeader
              .Include(x => x.FK_INV_StockTransactionHeader_SET_DocumentType)
              .Include(x => x.FK_INV_StockTransactionHeader_SET_Location_To)
              .Where(x => x.CompanyID == CurrentCompanyID && grnDocTypeIds.Contains(x.DocumentTypeID) && x.Status == true);

            if (status != "ALL" && short.TryParse(status, out short sc))
                query = query.Where(x => x.StatusCode == sc);

            var list = await query.OrderByDescending(x => x.DocumentDate).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(list);
        }

        // CREATE - PO Select karega pehle
        public async Task<IActionResult> Create(int? poId)
        {
            // Approved PO list jo fully received nahi
            var poList = await _context.AP_PurchaseOrderHeader
              .Include(x => x.Vendor)
              .Where(x => x.CompanyID == CurrentCompanyID && x.StatusCode == (short)POStatusCode.Approved && x.ClosedCode != (short)POClosedCode.ClosedForReceiving)
              .ToListAsync();

            var vm = new StockTransactionViewModel
            {
                Header = new INV_StockTransactionHeader
                {
                    DocumentDate = DateTime.Now,
                    CompanyID = CurrentCompanyID,
                    FiscalID = CurrentFiscalID,
                    StatusCode = (short)TransactionStatus.InProcess
                },
                Details = new List<INV_StockTransactionDetail>()
            };

            // Auto Doc No for GRN
            var grnDocType = await _context.SET_DocumentType.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.DocumentType == "GRN");
            if (grnDocType != null)
            {
                var maxNo = await _context.INV_StockTransactionHeader
                  .Where(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == grnDocType.DocumentTypeID)
                  .MaxAsync(x => (int?)x.DocumentNo) ?? 0;
                vm.Header.DocumentNo = maxNo + 1;
                vm.Header.DocumentTypeID = grnDocType.DocumentTypeID;
                vm.SelectedDocTypeID = grnDocType.DocumentTypeID;
            }

            if (poId.HasValue)
            {
                var po = await _context.AP_PurchaseOrderHeader.Include(x => x.Details).FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == poId);
                if (po != null)
                {
                    vm.Header.Remarks = $"GRN Against PO-{po.DocumentNumber}";
                    foreach (var d in po.Details)
                    {
                        // Kitni qty already received hai?
                        var receivedQty = await _context.INV_StockTransactionDetail
                          .Where(x => x.CompanyID == CurrentCompanyID && x.SourceHeaderID == po.POHeaderID && x.SourceDetailID == d.PODetailID)
                          .SumAsync(x => (decimal?)x.Quantity) ?? 0;
                        var pendingQty = d.Quantity - receivedQty;
                        if (pendingQty > 0)
                        {
                            vm.Details.Add(new INV_StockTransactionDetail
                            {
                                ItemID = d.ItemID,
                                Quantity = pendingQty,
                                Rate = d.Rate,
                                Amount = pendingQty * d.Rate,
                                SourceHeaderID = po.POHeaderID,
                                SourceDetailID = d.PODetailID,
                                SourceDocumentTypeID = po.DocumentTypeID,
                                SourceCode = po.DocumentNumber
                            });
                        }
                    }
                    ViewBag.SelectedPOId = poId;
                }
            }

            vm = await ReloadVM(vm);
            ViewBag.POList = poList;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionViewModel vm)
        {
            vm.Details.RemoveAll(x => x.ItemID == 0 || x.Quantity <= 0);
            if (!vm.Details.Any())
            {
                ModelState.AddModelError("", "At least one item required");
                return View(await ReloadVM(vm));
            }

            using var tran = await _context.Database.BeginTransactionAsync();
            try
            {
                var maxId = await _context.INV_StockTransactionHeader.Where(x => x.CompanyID == CurrentCompanyID).MaxAsync(x => (long?)x.StockTransactionID) ?? 0;
                var maxDocNo = await _context.INV_StockTransactionHeader.Where(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == vm.Header.DocumentTypeID).MaxAsync(x => (int?)x.DocumentNo) ?? 0;

                vm.Header.StockTransactionID = maxId + 1;
                vm.Header.DocumentNo = maxDocNo + 1;
                vm.Header.CompanyID = CurrentCompanyID;
                vm.Header.FiscalID = CurrentFiscalID;
                vm.Header.CreateDate = DateTime.Now;
                vm.Header.CreatedBy = CurrentUserID;
                vm.Header.CreatedByValue = CurrentUserName;
                vm.Header.Status = true;
                vm.Header.StatusCode = (short)TransactionStatus.InProcess;
                vm.Header.TotalAmount = vm.Details.Sum(x => x.Quantity * x.Rate);

                _context.INV_StockTransactionHeader.Add(vm.Header);
                await _context.SaveChangesAsync();

                long detId = 1;
                foreach (var d in vm.Details)
                {
                    d.CompanyID = CurrentCompanyID;
                    d.StockTransactionID = vm.Header.StockTransactionID;
                    d.TransactionDetailID = detId++;
                    d.Quantity = Math.Abs(d.Quantity); // GRN always IN
                    d.Amount = d.Quantity * d.Rate;
                    d.Status = true;
                    d.CreateDate = DateTime.Now;
                    d.CreatedBy = CurrentUserID;
                    d.CreatedByValue = CurrentUserName;
                }
                _context.INV_StockTransactionDetail.AddRange(vm.Details);
                await _context.SaveChangesAsync();
                await tran.CommitAsync();

                // Check if PO is fully received -> update ClosedCode
                if (vm.Details.First().SourceHeaderID.HasValue)
                {
                    var poHeaderId = vm.Details.First().SourceHeaderID.Value;
                    var poDetails = await _context.AP_PurchaseOrderDetail.Where(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == poHeaderId).ToListAsync();
                    bool allReceived = true;
                    foreach (var pd in poDetails)
                    {
                        var recQty = await _context.INV_StockTransactionDetail.Where(x => x.CompanyID == CurrentCompanyID && x.SourceHeaderID == poHeaderId && x.SourceDetailID == pd.PODetailID).SumAsync(x => (decimal?)x.Quantity) ?? 0;
                        if (recQty < pd.Quantity) allReceived = false;
                    }
                    var poHeader = await _context.AP_PurchaseOrderHeader.FindAsync(CurrentCompanyID, poHeaderId);
                    if (poHeader != null)
                    {
                        poHeader.ClosedCode = allReceived ? (short)POClosedCode.ClosedForReceiving : (short)POClosedCode.PartiallyReceived;
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["Success"] = $"GRN {vm.Header.DocumentNo} saved";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                ModelState.AddModelError("", ex.Message);
                return View(await ReloadVM(vm));
            }
        }

        public async Task<IActionResult> Details(long id)
        {
            var header = await _context.INV_StockTransactionHeader
              .Include(x => x.FK_INV_StockTransactionHeader_SET_DocumentType)
              .Include(x => x.FK_INV_StockTransactionHeader_SET_Location_To)
              .Include(x => x.StockTransactionDetail).ThenInclude(d => d.Item)
              .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);
            if (header == null) return NotFound();
            return View(header);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(long id)
        {
            var h = await _context.INV_StockTransactionHeader.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);
            if (h == null) return NotFound();
            h.StatusCode = (short)TransactionStatus.Approved;
            h.UpdateDate = DateTime.Now; h.UpdatedBy = CurrentUserID; h.UpdatedByValue = CurrentUserName;
            await _context.SaveChangesAsync();
            TempData["Success"] = "GRN Approved"; return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var h = await _context.INV_StockTransactionHeader.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.StockTransactionID == id);
            if (h == null) return NotFound();
            if (h.StatusCode != (short)TransactionStatus.InProcess) { TempData["Error"] = "Only In-Process can be deleted"; return RedirectToAction("Index"); }
            h.Status = false; await _context.SaveChangesAsync();
            TempData["Success"] = "GRN deleted"; return RedirectToAction("Index");
        }

        private async Task<StockTransactionViewModel> ReloadVM(StockTransactionViewModel vm)
        {
            vm.DocumentTypes = await _context.SET_DocumentType.Where(x => x.CompanyID == CurrentCompanyID && x.IsActive).ToListAsync();
            vm.Locations = await _context.SET_Location.Where(x => x.CompanyID == CurrentCompanyID && x.IsActive).ToListAsync();
            vm.Items = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.IsActive).ToListAsync();
            if (vm.Details.Count == 0) vm.Details = new List<INV_StockTransactionDetail> { new() };
            return vm;
        }

        public async Task<JsonResult> GetPOItems(int poId)
        {
            var details = await _context.AP_PurchaseOrderDetail.Where(x => x.CompanyID == CurrentCompanyID && x.POHeaderID == poId).ToListAsync();
            var result = new List<object>();
            foreach (var d in details)
            {
                var recQty = await _context.INV_StockTransactionDetail.Where(x => x.CompanyID == CurrentCompanyID && x.SourceHeaderID == poId && x.SourceDetailID == d.PODetailID).SumAsync(x => (decimal?)x.Quantity) ?? 0;
                result.Add(new { d.ItemID, d.Quantity, d.Rate, received = recQty, pending = d.Quantity - recQty, d.PODetailID, d.Remarks });
            }
            return Json(result);
        }
    }
}