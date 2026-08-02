using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Data;
using KerashineERP.ViewModels;
using KerashineERP.Models.Purchase;

namespace KerashineERP.Controllers.Purchase
{
    [Authorize]
    public class BusinessPartnerController : BaseController
    {
        private readonly AppDbContext _context;
        public BusinessPartnerController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var list = await _context.AR_SET_Customer.Where(x => x.CompanyID == CurrentCompanyID).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new BusinessPartnerVM
            {
                AllTypes = await _context.AR_SET_BusinessPartnerType.Where(x => x.IsActive).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessPartnerVM vm)
        {
            if (ModelState.IsValid)
            {
                // 1. CustomerID Generate
                var maxCustId = await _context.AR_SET_Customer
                   .Where(x => x.CompanyID == CurrentCompanyID)
                   .MaxAsync(x => (int?)x.CustomerID)?? 0;

                vm.Customer.CustomerID = maxCustId + 1;
                vm.Customer.CompanyID = CurrentCompanyID;
                vm.Customer.Id = maxCustId + 1;
                vm.Customer.CustomerCode = $"BP-{(maxCustId + 1).ToString("D4")}";

                _context.AR_SET_Customer.Add(vm.Customer);
                await _context.SaveChangesAsync();

                // 2. Mapping Table mein insert
                foreach(var typeId in vm.SelectedTypeIds)
                {
                    var map = new AR_SET_BusinessPartnerTypes
                    {
                        CompanyID = CurrentCompanyID,
                        BusinessPartnerID = vm.Customer.CustomerID, // CustomerID jayega
                        TypeCode = (short)typeId // Type ID jayega
                    };
                    _context.AR_SET_BusinessPartnerTypes.Add(map);
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Business Partner created successfully";
                return RedirectToAction(nameof(Index));
            }
            vm.AllTypes = await _context.AR_SET_BusinessPartnerType.ToListAsync();
            return View(vm);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var customer = await _context.AR_SET_Customer
            .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.CustomerID == id);
            if (customer == null) return NotFound();

            var selectedTypes = await _context.AR_SET_BusinessPartnerTypes
            .Where(x => x.CompanyID == CurrentCompanyID && x.BusinessPartnerID == id)
            .Select(x => (int)x.TypeCode).ToListAsync();

            var vm = new BusinessPartnerVM
            {
                Customer = customer,
                SelectedTypeIds = selectedTypes,
                AllTypes = await _context.AR_SET_BusinessPartnerType.Where(x => x.IsActive).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BusinessPartnerVM vm)
        {
            if (id!= vm.Customer.CustomerID) return NotFound();
            if (ModelState.IsValid)
            {
                vm.Customer.CompanyID = CurrentCompanyID;
                _context.Update(vm.Customer);

                // Mapping table delete and re-insert
                var oldMaps = _context.AR_SET_BusinessPartnerTypes
                .Where(x => x.CompanyID == CurrentCompanyID && x.BusinessPartnerID == id);
                _context.AR_SET_BusinessPartnerTypes.RemoveRange(oldMaps);

                foreach (var typeId in vm.SelectedTypeIds)
                {
                    _context.AR_SET_BusinessPartnerTypes.Add(new AR_SET_BusinessPartnerTypes
                    {
                        CompanyID = CurrentCompanyID,
                        BusinessPartnerID = id,
                        TypeCode = (short)typeId
                    });
                }
                await _context.SaveChangesAsync();
                TempData["Success"] = "Business Partner updated successfully";
                return RedirectToAction(nameof(Index));
            }
            vm.AllTypes = await _context.AR_SET_BusinessPartnerType.ToListAsync();
            return View(vm);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var customer = await _context.AR_SET_Customer
            .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.CustomerID == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.AR_SET_Customer
            .FirstOrDefaultAsync(x => x.CompanyID == CurrentCompanyID && x.CustomerID == id);
            if (customer!= null)
            {
                var maps = _context.AR_SET_BusinessPartnerTypes
                .Where(x => x.CompanyID == CurrentCompanyID && x.BusinessPartnerID == id);
                _context.AR_SET_BusinessPartnerTypes.RemoveRange(maps);
                _context.AR_SET_Customer.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Business Partner deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}