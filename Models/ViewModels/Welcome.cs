using KerashineERP.Models.Master;

namespace KerashineERP.Models.ViewModels
{
    public class Welcome
    {
        public int BusinessID { get; set; }
        public int CompanyID { get; set; }
        public int FiscalID { get; set; }

        public List<SET_Business> Businesses { get; set; } = new();
        public List<SET_Company> Companies { get; set; } = new();
        public List<SET_Fiscal> Fiscals { get; set; } = new();
    }
}