using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Inventory;
using System.Security.Claims;

namespace KerashineERP.Controllers.Inventory
{
    [Authorize]
    public class ItemCategoryController : BaseController
    {
        private readonly AppDbContext _context;

        public ItemCategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ItemCategory
        public async Task<IActionResult> Index()
        {
            return View(await _context.INV_SET_Category.Where(x => x.Status == true && x.CompanyID == CurrentCompanyID).ToListAsync());
        }

        // GET: ItemCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(INV_SET_Category category)
        {
            if (ModelState.IsValid)
            {
                // ✅ Manual CategoryId generate karo
                var maxId = await _context.INV_SET_Category
                 .Where(x => x.CompanyID == CurrentCompanyID)
                 .MaxAsync(x => (int?)x.CategoryId)?? 0;
                
                category.CategoryId = maxId + 1;
                category.Status = true;
                category.CreateDate = DateTime.Now;
                category.CreatedBy = CurrentUserID;
                category.CreatedByValue = CurrentUserName;
                category.CompanyID = CurrentCompanyID;

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: ItemCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.INV_SET_Category.FindAsync(CurrentCompanyID, id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: ItemCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, INV_SET_Category category)
        {
            if (id!= category.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    category.Status = true;
                    category.UpdateDate = DateTime.Now;
                    category.UpdatedBy = CurrentUserID;
                    category.UpdatedByValue = CurrentUserName;
                    category.CompanyID = CurrentCompanyID;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Category updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.INV_SET_Category.Any(e => e.CategoryId == id && e.CompanyID == CurrentCompanyID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: ItemCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.INV_SET_Category.FindAsync(CurrentCompanyID, id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: ItemCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.INV_SET_Category.FindAsync(CurrentCompanyID, id);
            if (category!= null)
            {
                category.Status = false; // Soft delete
                category.UpdateDate = DateTime.Now;
                category.UpdatedBy = CurrentUserID;
                category.UpdatedByValue = CurrentUserName;
                
                // ❌ Remove mat karo - soft delete hai
                // _context.INV_SET_Category.Remove(category);
                
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}