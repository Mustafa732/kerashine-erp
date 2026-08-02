using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Purchase;

namespace KerashineERP.Controllers.Purchase
{
    [Authorize]
    public class BusinessPartnerTypeController : BaseController
    {
        private readonly AppDbContext _context;
        public BusinessPartnerTypeController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            return View(await _context.AR_SET_BusinessPartnerType.OrderBy(x => x.BusinessPartnerID).ToListAsync());
        }

        public IActionResult Create() => View(new AR_SET_BusinessPartnerType());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AR_SET_BusinessPartnerType model)
        {
            if (ModelState.IsValid)
            {
                var maxId = await _context.AR_SET_BusinessPartnerType.MaxAsync(x => (int?)x.BusinessPartnerID)?? 0;
                model.BusinessPartnerID = maxId + 1;
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Business Partner Type created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var data = await _context.AR_SET_BusinessPartnerType.FindAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AR_SET_BusinessPartnerType model)
        {
            if (id!= model.BusinessPartnerID) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Business Partner Type updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var data = await _context.AR_SET_BusinessPartnerType.FindAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var data = await _context.AR_SET_BusinessPartnerType.FindAsync(id);
            if (data!= null)
            {
                _context.AR_SET_BusinessPartnerType.Remove(data);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Business Partner Type deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}