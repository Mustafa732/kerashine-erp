using KerashineERP.Data;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.Production;
using KerashineERP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerashineERP.Controllers.Production
{
    public class ProductionReceiptController : BaseController
    {
        private readonly AppDbContext _context;
        public ProductionReceiptController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var list = await _context.PRO_ProductionReceiptHeader
             .Include(x => x.ProductionOrder).ThenInclude(x => x!.Product)
             .Where(x => x.CompanyID == CurrentCompanyID && x.Status == true)
             .OrderByDescending(x => x.ReceiptID).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ProductionReceiptViewModel
            {
                ProductionOrders = await _context.PRO_ProductionOrderHeader
               .Include(x => x.Product)
               .Where(x => x.CompanyID == CurrentCompanyID && x.StatusCode == 1 && x.Status == true)
               .ToListAsync()
            };
            vm.Header.ReceiptDate = DateTime.Now;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductionReceiptViewModel vm)
        {
            int nextId = (await _context.PRO_ProductionReceiptHeader.Where(x => x.CompanyID == CurrentCompanyID).MaxAsync(x => (int?)x.ReceiptID)??0)+1;
            vm.Header.CompanyID = CurrentCompanyID;
            vm.Header.ReceiptID = nextId;
            vm.Header.ReceiptNo = $"PR-{nextId:D4}";
            vm.Header.CreatedBy = CurrentUserID;
            vm.Header.CreateDate = DateTime.Now;
            vm.Header.CreatedByValue = CurrentUserName;
            vm.Header.Status = true;
            _context.PRO_ProductionReceiptHeader.Add(vm.Header);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Receipt Created";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var h = await _context.PRO_ProductionReceiptHeader
             .Include(x => x.ProductionOrder).ThenInclude(x => x!.Product)
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ReceiptID == id);
            return View(h);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var header = await _context.PRO_ProductionReceiptHeader
             .Include(x => x.ProductionOrder)
             .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.ReceiptID == id);

            if (header == null) return NotFound();

            long nextStockId = (await _context.INV_StockTransactionHeader.MaxAsync(x => (long?)x.StockTransactionID)??0)+1;
            int nextDocNo = (await _context.INV_StockTransactionHeader.Where(x => x.CompanyID == CurrentCompanyID && x.DocumentTypeID == 2).MaxAsync(x => (int?)x.DocumentNo)??0)+1;

            var stockHeader = new INV_StockTransactionHeader
            {
                CompanyID = CurrentCompanyID,
                StockTransactionID = nextStockId,
                DocumentNo = nextDocNo,
                DocumentDate = DateTime.Now,
                DocumentTypeID = 2, // ADJUSTMENT_IN - FG Production
                FromLocationID = null,
                ToLocationID = 1, // FG Location - abhi 1 hi hai
                FiscalID = CurrentFiscalID,
                Remarks = $"FG Receipt {header.ReceiptNo} against PO-{header?.ProductionOrder?.ProductionOrderNo}",
                StatusCode = 1,
                CreatedBy = CurrentUserID,
                CreateDate = DateTime.Now,
                CreatedByValue = CurrentUserName,
                Status = true
            };
            _context.INV_StockTransactionHeader.Add(stockHeader);
            await _context.SaveChangesAsync();

            _context.INV_StockTransactionDetail.Add(new INV_StockTransactionDetail
            {
                CompanyID = CurrentCompanyID,
                StockTransactionID = nextStockId,
                TransactionDetailID = 1,
                ItemID = header!.ProductionOrder!.ProductID,
                Quantity = header!.ReceivedQty,
                Rate = 0, Amount = 0,
                CreatedBy = CurrentUserID,
                CreateDate = DateTime.Now,
                CreatedByValue = CurrentUserName,
                Status = true
            });

            header.StatusCode = 1;
            header.UpdatedBy = CurrentUserID;
            header.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Approved & FG Stock Added";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var h = await _context.PRO_ProductionReceiptHeader.Include(x=>x.ProductionOrder).FirstOrDefaultAsync(x=>x.CompanyID==CurrentCompanyID && x.ReceiptID==id);
            return View(h);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var h = await _context.PRO_ProductionReceiptHeader.FirstOrDefaultAsync(x=>x.CompanyID==CurrentCompanyID && x.ReceiptID==id);
            if(h!=null){ h.Status=false; await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}