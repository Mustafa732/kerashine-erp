using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.Models.Master;

namespace KerashineERP.Controllers
{
    [Authorize]
    public class DocumentTypeController : BaseController
    {
        private readonly AppDbContext _context;
        public DocumentTypeController(AppDbContext context) => _context = context;

        // GET: DocumentType
        public async Task<IActionResult> Index()
        {
            return View(await _context.SET_DocumentType
               .Where(x => x.CompanyID == CurrentCompanyID)
               .OrderBy(x => x.DocumentTypeID)
               .ToListAsync());
        }

        // GET: Create
        public IActionResult Create() => View(new SET_DocumentType());

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SET_DocumentType doc)
        {
            if (ModelState.IsValid)
            {
                var maxId = await _context.SET_DocumentType
                   .Where(x => x.CompanyID == CurrentCompanyID)
                   .MaxAsync(x => (int?)x.DocumentTypeID)?? 0;

                doc.DocumentTypeID = maxId + 1;
                doc.CompanyID = CurrentCompanyID;

                _context.Add(doc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document Type created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(doc);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var doc = await _context.SET_DocumentType.FindAsync(CurrentCompanyID, id);
            if (doc == null) return NotFound();
            return View(doc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SET_DocumentType doc)
        {
            if (id!= doc.DocumentTypeID) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(doc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document Type updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(doc);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var doc = await _context.SET_DocumentType.FindAsync(CurrentCompanyID, id);
            if (doc == null) return NotFound();
            return View(doc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doc = await _context.SET_DocumentType.FindAsync(CurrentCompanyID, id);
            if (doc!= null)
            {
                _context.SET_DocumentType.Remove(doc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Document Type deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}