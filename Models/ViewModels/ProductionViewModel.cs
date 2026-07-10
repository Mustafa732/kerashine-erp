using KerashineERP.Models.Production;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.ViewModels
{
    public class ProductionViewModel
    {
        public PRO_ProductionHeader Header { get; set; } = new();
        public List<PRO_ProductionDetail> Details { get; set; } = new() { new() };
        
        // Dropdowns ke liye
        public List<INV_Item> Products { get; set; } = new();
        public List<INV_Item> RawMaterials { get; set; } = new();
    }
}