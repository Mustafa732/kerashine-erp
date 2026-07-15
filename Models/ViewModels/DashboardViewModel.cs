namespace KerashineERP.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalItems { get; set; }
        public int RawMaterialCount { get; set; }
        public int PackagingMaterialCount { get; set; }
        public int FinishedGoodsCount { get; set; }
        public decimal TotalStockValue { get; set; }
        public List<ItemStockViewModel> LowStockItems { get; set; } = new();
    }

    // Naya ViewModel for Low Stock
    public class ItemStockViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public decimal CurrentStock { get; set; }
        public string StockUOM { get; set; }
        public decimal ReorderLevel { get; set; }
    }
}