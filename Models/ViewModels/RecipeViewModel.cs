using KerashineERP.Models.Inventory;
using KerashineERP.Models.Production;

namespace KerashineERP.Models.ViewModels
{
    public class RecipeViewModel
    {
        public PRO_RecipeHeader Header { get; set; } = new();
        public List<PRO_RecipeDetail> Details { get; set; } = new() { new PRO_RecipeDetail() };
        public List<INV_Item> FGItems { get; set; } = new();
        public List<INV_Item> Materials { get; set; } = new();
        public List<INV_SET_UOM> UOMs { get; set; } = new();
    }
}