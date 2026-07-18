using KerashineERP.Models.Inventory;
using KerashineERP.Models.Production;

namespace KerashineERP.Models.ViewModels
{
    public class ProductionOrderViewModel
    {
        public PRO_ProductionOrderHeader Header { get; set; } = new();
        public List<PRO_ProductionOrderDetail> Details { get; set; } = new();
        public List<INV_Item> FGItems { get; set; } = new();
        public List<PRO_RecipeHeader> Recipes { get; set; } = new();
        public List<INV_Item> Materials { get; set; } = new();
        public List<INV_SET_UOM> UOMs { get; set; } = new();
    }
}