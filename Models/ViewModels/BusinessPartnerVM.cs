using KerashineERP.Models.Purchase;

namespace KerashineERP.ViewModels
{
    public class BusinessPartnerVM
    {
        public AR_SET_Customer Customer { get; set; } = new();
        public List<int> SelectedTypeIds { get; set; } = new(); // e.g. [1,2]
        public List<AR_SET_BusinessPartnerType> AllTypes { get; set; } = new();
    }
}