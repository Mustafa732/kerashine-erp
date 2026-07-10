using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Controllers.Inventory
{
    [Authorize]
    public class UOMController : BaseController
    {
        private readonly AppDbContext _context;

        public UOMController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.INV_SET_UOM.Where(x => x.Status == true && x.CompanyID == CurrentCompanyID)
              .OrderByDescending(x => x.CreateDate)
              .ToListAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(INV_SET_UOM uom)
        {
            if (ModelState.IsValid)
            {
                // ✅ Manual UOMId generate karo
                var maxId = await _context.INV_SET_UOM
                 .Where(x => x.CompanyID == CurrentCompanyID)
                 .MaxAsync(x => (int?)x.UOMId)?? 0;
                
                uom.UOMId = maxId + 1;
                uom.Status = true;
                uom.CreateDate = DateTime.Now;
                uom.CreatedBy = CurrentUserID;
                uom.CreatedByValue = CurrentUserName;
                uom.CompanyID = CurrentCompanyID;

                _context.Add(uom);
                await _context.SaveChangesAsync();
                TempData["Success"] = "UOM created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(uom);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var uom = await _context.INV_SET_UOM.FindAsync(CurrentCompanyID, id);
            if (uom == null) return NotFound();
            return View(uom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, INV_SET_UOM uom)
        {
            if (id!= uom.UOMId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    uom.Status = true;
                    uom.UpdateDate = DateTime.Now;
                    uom.UpdatedBy = CurrentUserID;
                    uom.UpdatedByValue = CurrentUserName;
                    uom.CompanyID = CurrentCompanyID;

                    _context.Update(uom);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "UOM updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.INV_SET_UOM.Any(e => e.UOMId == id && e.CompanyID == CurrentCompanyID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(uom);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var uom = await _context.INV_SET_UOM.FindAsync(CurrentCompanyID, id);
            if (uom == null) return NotFound();
            return View(uom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uom = await _context.INV_SET_UOM.FindAsync(CurrentCompanyID, id);
            if (uom!= null)
            {
                uom.Status = false;
                uom.UpdateDate = DateTime.Now;
                uom.UpdatedBy = CurrentUserID;
                uom.UpdatedByValue = CurrentUserName;
                
                await _context.SaveChangesAsync();
                TempData["Success"] = "UOM deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}