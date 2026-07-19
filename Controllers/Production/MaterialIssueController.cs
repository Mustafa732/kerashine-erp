using KerashineERP.Data;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerashineERP.Controllers.Production
{
    public class MaterialIssueController : BaseController
    {
        private readonly AppDbContext _context;
        public MaterialIssueController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var list = await _context.PRO_MaterialIssueHeader
               .Include(x => x.ProductionOrder!).ThenInclude(x => x.Product)
               .Where(x => x.CompanyID == CurrentCompanyID && x.Status == true)
               .OrderByDescending(x => x.IssueID).ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var details = await _context.PRO_ProductionOrderDetail
               .Include(x => x.Material)
               .Where(x => x.CompanyID == CurrentCompanyID && x.ProductionOrderID == orderId)
               .Select(x => new {
                    materialID = x.MaterialID,
                    materialCode = x.Material!.ItemCode,
                    materialName = x.Material.ItemName,
                    requiredQty = x.RequiredQty,
                    uomId = x.UOMId,
                    uomCode = x.UOM!.UOMCode,
                    wastage = x.WastagePercent
                }).ToListAsync();
            return Json(details);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new MaterialIssueViewModel
            {
                ProductionOrders = await _context.PRO_ProductionOrderHeader
                   .Include(x => x.Product)
                   .Where(x => x.CompanyID == CurrentCompanyID && x.StatusCode == 1 && x.Status == true)
                   .ToListAsync()
            };
            vm.Header.IssueDate = DateTime.Now;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaterialIssueViewModel vm)
        {
            int nextId = (await _context.PRO_MaterialIssueHeader.Where(x => x.CompanyID == CurrentCompanyID).MaxAsync(x => (int?)x.IssueID)?? 0) + 1;
            vm.Header.CompanyID = CurrentCompanyID;
            vm.Header.IssueID = nextId;
            vm.Header.IssueNo = $"MI-{nextId:D4}";
            vm.Header.CreatedBy = CurrentUserID;
            vm.Header.CreateDate = DateTime.Now;
            vm.Header.CreatedByValue = CurrentUserName;
            vm.Header.Status = true;

            _context.PRO_MaterialIssueHeader.Add(vm.Header);

            int dId = 1;
            foreach (var d in vm.Details.Where(x => x.MaterialID > 0))
            {
                d.CompanyID = CurrentCompanyID;
                d.IssueID = nextId;
                d.IssueDetailID = dId++;
                d.CreatedBy = CurrentUserID;
                d.CreateDate = DateTime.Now;
                d.CreatedByValue = CurrentUserName;
                d.Status = true;
                _context.PRO_MaterialIssueDetail.Add(d);
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Material Issue Created";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var header = await _context.PRO_MaterialIssueHeader
               .Include(x => x.ProductionOrder!).ThenInclude(x => x.Product)
               .Include(x => x.IssueDetails!).ThenInclude(x => x.Material)
               .Include(x => x.IssueDetails!).ThenInclude(x => x.UOM)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.IssueID == id);
            return View(header);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var header = await _context.PRO_MaterialIssueHeader
        .Include(x => x.IssueDetails)
        .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.IssueID == id);

            if (header == null) return NotFound();
            if (header.StatusCode == 1) { TempData["Error"] = "Already Approved"; return RedirectToAction(nameof(Details), new { id }); }

            var fiscal = await _context.SET_Fiscal.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.FiscalID == CurrentFiscalID);
            if (fiscal == null) { TempData["Error"] = "Active Fiscal Not Found"; return RedirectToAction(nameof(Details), new { id }); }

            long nextStockId = (await _context.INV_StockTransactionHeader.Where(x => x.CompanyID == CurrentCompanyID).MaxAsync(x => (long?)x.StockTransactionID)?? 0) + 1;
            int nextDocNo = (await _context.INV_StockTransactionHeader.Where(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == 3).MaxAsync(x => (int?)x.DocumentNo)?? 0) + 1;

            // --- HEADER ---
            var stockHeader = new INV_StockTransactionHeader
            {
                CompanyID = CurrentCompanyID,
                StockTransactionID = nextStockId,
                DocumentNo = nextDocNo,
                DocumentDate = DateTime.Now,
                DocumentTypeID = 3, // ADJUSTMENT_OUT - Consumption
                FromLocationID = 1, // Sirf 1 hi location hai tumhare pass, ye valid hai
                ToLocationID = null, // ADJUSTMENT_OUT mein ToLocation null rakho
                FiscalID = fiscal.FiscalID,
                Remarks = $"Material Issue {header.IssueNo} for PO-{header.ProductionOrderID}",
                StatusCode = 1,
                CreatedBy = CurrentUserID,
                CreateDate = DateTime.Now,
                CreatedByValue = CurrentUserName,
                Status = true
            };
            _context.INV_StockTransactionHeader.Add(stockHeader);
            await _context.SaveChangesAsync(); // Pehle Header save karo taake FK error na aaye

            // --- DETAILS ---
            int dId = 1;
            foreach (var item in header.IssueDetails!.Where(x => x.Status == true))
            {
                _context.INV_StockTransactionDetail.Add(new INV_StockTransactionDetail
                {
                    CompanyID = CurrentCompanyID,
                    StockTransactionID = nextStockId,
                    TransactionDetailID = dId++,
                    ItemID = item.MaterialID,
                    Quantity = item.IssuedQty,
                    Rate = 0,
                    Amount = 0,
                    CreatedBy = CurrentUserID,
                    CreateDate = DateTime.Now,
                    CreatedByValue = CurrentUserName,
                    Status = true
                });
            }

            header.StatusCode = 1;
            header.UpdatedBy = CurrentUserID;
            header.UpdateDate = DateTime.Now;
            header.UpdatedByValue = CurrentUserName;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Approved & Stock Deducted Successfully";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var header = await _context.PRO_MaterialIssueHeader.Include(x => x.ProductionOrder).FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.IssueID == id);
            return View(header);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var header = await _context.PRO_MaterialIssueHeader.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.IssueID == id);
            if (header!= null) { header.Status = false; await _context.SaveChangesAsync(); }
            TempData["Success"] = "Issue Deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}