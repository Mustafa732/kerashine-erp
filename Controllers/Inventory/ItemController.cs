using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Controllers.Inventory
{
    [Authorize]
    public class ItemController : BaseController
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string type = "ALL")
        {
            var query = _context.INV_Item
            .Where(x => x.Status == true && x.CompanyID == CurrentCompanyID)
            .Include(x => x.Category)
            .Include(x => x.UOM)
            .AsQueryable();

            if (type!= "ALL")
                query = query.Where(x => x.ItemType == type);

            var data = await query.OrderByDescending(x => x.CreateDate).ToListAsync();
            ViewBag.ItemType = type;
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.INV_SET_Category.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "CategoryId", "CategoryName");
            ViewBag.UOMId = new SelectList(_context.INV_SET_UOM.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "UOMId", "UOMName");
            ViewBag.ItemTypes = new SelectList(new[] 
            { 
                new { Value = "RM", Text = "Raw Material" },
                new { Value = "PM", Text = "Packaging Material" },
                new { Value = "FG", Text = "Finished Good" }
            }, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(INV_Item item)
        {
            if (ModelState.IsValid)
            {
                // ✅ Manual ItemId generate karo
                var maxId = await _context.INV_Item
                    .Where(x => x.CompanyID == CurrentCompanyID)
                    .MaxAsync(x => (int?)x.ItemId)?? 0;
                
                item.ItemId = maxId + 1;
                item.Status = true;
                item.CreateDate = DateTime.Now;
                item.CreatedBy = CurrentUserID;
                item.CreatedByValue = CurrentUserName;
                item.CompanyID = CurrentCompanyID;

                _context.Add(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item created successfully";
                return RedirectToAction(nameof(Index), new { type = item.ItemType });
            }

            ViewBag.CategoryId = new SelectList(_context.INV_SET_Category.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "CategoryId", "CategoryName", item.CategoryId);
            ViewBag.UOMId = new SelectList(_context.INV_SET_UOM.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "UOMId", "UOMName", item.UOMId);
            ViewBag.ItemTypes = new SelectList(new[] 
            { 
                new { Value = "RM", Text = "Raw Material" },
                new { Value = "PM", Text = "Packaging Material" },
                new { Value = "FG", Text = "Finished Good" }
            }, "Value", "Text", item.ItemType);
            return View(item);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.INV_Item.FindAsync(CurrentCompanyID, id);
            if (item == null) return NotFound();
            
            ViewBag.CategoryId = new SelectList(_context.INV_SET_Category.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "CategoryId", "CategoryName", item.CategoryId);
            ViewBag.UOMId = new SelectList(_context.INV_SET_UOM.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "UOMId", "UOMName", item.UOMId);
            ViewBag.ItemTypes = new SelectList(new[] 
            { 
                new { Value = "RM", Text = "Raw Material" },
                new { Value = "PM", Text = "Packaging Material" },
                new { Value = "FG", Text = "Finished Good" }
            }, "Value", "Text", item.ItemType);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, INV_Item item)
        {
            if (id!= item.ItemId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingItem = await _context.INV_Item.FindAsync(CurrentCompanyID, id);
                    if (existingItem == null) return NotFound();

                    existingItem.ItemCode = item.ItemCode;
                    existingItem.ItemName = item.ItemName;
                    existingItem.ItemType = item.ItemType;
                    existingItem.ItemSize = item.ItemSize;
                    existingItem.SizeUOM = item.SizeUOM;
                    existingItem.CategoryId = item.CategoryId;
                    existingItem.UOMId = item.UOMId;
                    existingItem.Description = item.Description;
                    existingItem.ReorderLevel = item.ReorderLevel;
                    existingItem.MinStock = item.MinStock;
                    existingItem.MaxStock = item.MaxStock;
                    existingItem.HsCode = item.HsCode;
                    existingItem.Barcode = item.Barcode;
                    existingItem.IsActive = item.IsActive;
                    existingItem.ItemImage = item.ItemImage;
                    existingItem.CompanyID = CurrentCompanyID;
                    
                    existingItem.UpdateDate = DateTime.Now;
                    existingItem.UpdatedBy = CurrentUserID;
                    existingItem.UpdatedByValue = CurrentUserName;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Item updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.INV_Item.Any(e => e.ItemId == id && e.CompanyID == CurrentCompanyID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index), new { type = item.ItemType });
            }
            ViewBag.CategoryId = new SelectList(_context.INV_SET_Category.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "CategoryId", "CategoryName", item.CategoryId);
            ViewBag.UOMId = new SelectList(_context.INV_SET_UOM.Where(x => x.IsActive && x.CompanyID == CurrentCompanyID), "UOMId", "UOMName", item.UOMId);
            ViewBag.ItemTypes = new SelectList(new[] 
            { 
                new { Value = "RM", Text = "Raw Material" },
                new { Value = "PM", Text = "Packaging Material" },
                new { Value = "FG", Text = "Finished Good" }
            }, "Value", "Text", item.ItemType);
            return View(item);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.INV_Item
           .Where(x => x.CompanyID == CurrentCompanyID)
           .Include(i => i.Category)
           .Include(i => i.UOM)
           .FirstOrDefaultAsync(i => i.ItemId == id);
                
            if (item == null) return NotFound();
            
            ViewBag.ItemTypes = new SelectList(new[] 
            { 
                new { Value = "RM", Text = "Raw Material" },
                new { Value = "PM", Text = "Packaging Material" },
                new { Value = "FG", Text = "Finished Good" }
            }, "Value", "Text", item.ItemType);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.INV_Item.FindAsync(CurrentCompanyID, id);
            if (item == null) return NotFound();

            item.Status = false; // Soft delete
            item.UpdateDate = DateTime.Now;
            item.UpdatedBy = CurrentUserID;
            item.UpdatedByValue = CurrentUserName;
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Item deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}