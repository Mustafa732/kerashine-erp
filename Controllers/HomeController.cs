using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KerashineERP.Data;
using KerashineERP.Models;
using KerashineERP.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KerashineERP.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context; // ye line missing thi
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize]
    public async Task<IActionResult> Welcome()
    {
        var model = new Welcome
        {
            Businesses = await _context.SET_Business.Where(x => x.IsActive).ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Welcome(Welcome model)
    {
        HttpContext.Session.SetInt32("BusinessID", model.BusinessID);
        HttpContext.Session.SetInt32("CompanyID", model.CompanyID);
        HttpContext.Session.SetInt32("FiscalID", model.FiscalID);

        // Company + Fiscal names bhi save kar lo display ke liye
        var company = _context.SET_Company.Find(model.CompanyID);
        var fiscal = _context.SET_Fiscal.FirstOrDefault(f => f.CompanyID == model.CompanyID && f.FiscalID == model.FiscalID);
        HttpContext.Session.SetString("CompanyName", company?.CompanyName?? "");
        HttpContext.Session.SetString("FiscalYear", fiscal?.FiscalYear?? "");

        return RedirectToAction("Index", "Home");
    }

    // Get Companies by Business - AJAX
    public async Task<JsonResult> GetCompaniesByBusiness(int businessId)
    {
        var companies = await _context.SET_Company
        .Where(c => c.BusinessID == businessId && c.IsActive)
        .Select(c => new { c.CompanyID, c.CompanyName })
        .ToListAsync();
        return Json(companies);
    }

    // Get Fiscals by Company - AJAX
    public async Task<JsonResult> GetFiscalsByCompany(int companyId)
    {
        var fiscals = await _context.SET_Fiscal
        .Where(f => f.CompanyID == companyId && f.IsActive)
        .OrderByDescending(f => f.IsCurrent)
        .Select(f => new { f.FiscalID, f.FiscalYear })
        .ToListAsync();
        return Json(fiscals);
    }
}
