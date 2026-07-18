using KerashineERP.Data;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.Production;
using KerashineERP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerashineERP.Controllers.Production
{
    public class RecipeController : BaseController
    {
        private readonly AppDbContext _context;
        public RecipeController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var list = await _context.PRO_RecipeHeader
            .Include(x => x.Product)
            .Where(x => x.CompanyID == CurrentCompanyID && x.Status == true)
            .OrderByDescending(x => x.RecipeID).ToListAsync();

            ViewBag.LockedRecipeIds = await _context.PRO_ProductionOrderHeader
            .Where(x => x.CompanyID == CurrentCompanyID && x.StatusCode == 1)
            .Select(x => x.RecipeID).Distinct().ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new RecipeViewModel
            {
                FGItems = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.ItemType == "FG" && x.IsActive && x.Status).ToListAsync(),
                Materials = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && (x.ItemType == "RM" || x.ItemType == "PM") && x.IsActive && x.Status).ToListAsync(),
                UOMs = await _context.INV_SET_UOM.Where(x => x.CompanyID == CurrentCompanyID && x.IsActive && x.Status).ToListAsync()
            };
            vm.Header.BatchSize = 100;
            vm.Header.BatchSizeUOM = "KG";
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RecipeViewModel vm)
        {
            int nextId = (_context.PRO_RecipeHeader.Where(x => x.CompanyID == CurrentCompanyID).Max(x => (int?)x.RecipeID)?? 0) + 1;

            vm.Header.CompanyID = CurrentCompanyID;
            vm.Header.RecipeID = nextId;
            vm.Header.RecipeCode = $"REC-{nextId:D4}";
            vm.Header.CreatedBy = CurrentUserID;
            vm.Header.CreateDate = DateTime.Now;
            vm.Header.CreatedByValue = CurrentUserName;
            vm.Header.Status = true;

            _context.PRO_RecipeHeader.Add(vm.Header);

            int detailId = 1;
            foreach (var d in vm.Details.Where(x => x.MaterialID > 0 && x.QuantityRequired > 0))
            {
                d.CompanyID = CurrentCompanyID;
                d.RecipeID = nextId;
                d.RecipeDetailID = detailId++;
                d.CreatedBy = CurrentUserID;
                d.CreateDate = DateTime.Now;
                d.CreatedByValue = CurrentUserName;
                d.Status = true;
                _context.PRO_RecipeDetail.Add(d);
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Recipe Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var header = await _context.PRO_RecipeHeader.Include(x => x.Product)
               .Include(x => x.RecipeDetails).ThenInclude(x => x.Material)
               .Include(x => x.RecipeDetails).ThenInclude(x => x.UOM)
               .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.RecipeID == id);
            return View(header);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if(await IsRecipeLocked(id)) { TempData["Error"]="Locked! Approved, Used In Production "; return RedirectToAction(nameof(Index)); }
            var header = await _context.PRO_RecipeHeader.Include(x => x.RecipeDetails).FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.RecipeID == id);
            if (header == null) return NotFound();
            var vm = new RecipeViewModel
            {
                Header = header,
                Details = header.RecipeDetails.ToList(),
                FGItems = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && x.ItemType == "FG" && x.IsActive && x.Status).ToListAsync(),
                Materials = await _context.INV_Item.Where(x => x.CompanyID == CurrentCompanyID && (x.ItemType == "RM" || x.ItemType == "PM") && x.IsActive && x.Status).ToListAsync(),
                UOMs = await _context.INV_SET_UOM.Where(x => x.CompanyID == CurrentCompanyID && x.IsActive && x.Status).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RecipeViewModel vm)
        {
            var existing = await _context.PRO_RecipeHeader.Include(x => x.RecipeDetails).FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.RecipeID == vm.Header.RecipeID);
            if (existing == null) return NotFound();

            existing.ProductID = vm.Header.ProductID;
            existing.RecipeName = vm.Header.RecipeName;
            existing.BatchSize = vm.Header.BatchSize;
            existing.BatchSizeUOM = vm.Header.BatchSizeUOM;
            existing.Remarks = vm.Header.Remarks;
            existing.UpdateDate = DateTime.Now;
            existing.UpdatedBy = CurrentUserID;
            existing.UpdatedByValue = CurrentUserName;


            _context.PRO_RecipeDetail.RemoveRange(existing.RecipeDetails);
            int detailId = 1;

            foreach (var d in vm.Details.Where(x => x.MaterialID > 0 && x.QuantityRequired > 0))
            {
                d.CompanyID = CurrentCompanyID; d.RecipeID = existing.RecipeID; d.RecipeDetailID = detailId++;
                d.CreatedBy = CurrentUserID; d.CreateDate = DateTime.Now; d.Status = true;
                _context.PRO_RecipeDetail.Add(d);
            }
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Recipe Updated";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if(await IsRecipeLocked(id)) { TempData["Error"]="Locked! Approved, Used In Production "; return RedirectToAction(nameof(Index)); }
            var header = await _context.PRO_RecipeHeader.Include(x => x.Product).FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.RecipeID == id);
            return View(header);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var header = await _context.PRO_RecipeHeader.FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.RecipeID == id);
            if (header!= null) { header.Status = false; header.IsActive = false; await _context.SaveChangesAsync(); }
            TempData["Success"] = "Recipe Deleted";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsRecipeLocked(int recipeId)
        {
            return await _context.PRO_ProductionOrderHeader
            .AnyAsync(x => x.CompanyID == CurrentCompanyID 
                            && x.RecipeID == recipeId 
                            && x.StatusCode == 1); // 1 = Approved
        }
    }
}