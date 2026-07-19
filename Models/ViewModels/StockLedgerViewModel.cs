namespace KerashineERP.Models.ViewModels
{
    public class StockLedgerItem
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemType { get; set; }
        public string? UOM { get; set; }
        public decimal OpeningQty { get; set; }
        public decimal InQty { get; set; }
        public decimal OutQty { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
    }
    public class StockLedgerReportVM
    {
        public DateTime FromDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public List<StockLedgerItem> Items { get; set; } = new();
        public string FilterType { get; set; } = "All";
    }
}