using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace KerashineERP.Controllers
{
    public class BaseController : Controller
    {
        protected int CurrentCompanyID => HttpContext.Session.GetInt32("CompanyID")?? 0;
        protected int CurrentFiscalID => HttpContext.Session.GetInt32("FiscalID")?? 0;

        protected Guid CurrentUserID 
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(userId, out Guid id) ? id : Guid.Empty;
            }
        }

        protected string CurrentUserName => User.Identity?.Name?? string.Empty;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value?.ToLower()?? "";

            // Login, SelectCompany, aur static files ko skip karo
            if (CurrentCompanyID == 0 && 
                !path.Contains("/account/login") && 
                !path.Contains("/home/welcome") &&
                !path.StartsWith("/css") &&
                !path.StartsWith("/js") &&
                !path.StartsWith("/lib"))
            {
                context.Result = RedirectToAction("Welcome", "Home");
            }
            base.OnActionExecuting(context);
        }
    }
}