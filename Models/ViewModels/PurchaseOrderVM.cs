using KerashineERP.Models.Master;
using KerashineERP.Models.Purchase;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.ViewModels
{
    public class PurchaseOrderVM
    {
        public AP_PurchaseOrderHeader Header { get; set; } = new();
        public List<AP_PurchaseOrderDetail> Details { get; set; } = new();

        // Dropdowns
        public List<SET_DocumentType> DocumentTypes { get; set; } = new();
        public List<AR_SET_Customer> Vendors { get; set; } = new();
        public List<INV_Item> Items { get; set; } = new();

        public int SelectedDocTypeID { get; set; }
    }
}