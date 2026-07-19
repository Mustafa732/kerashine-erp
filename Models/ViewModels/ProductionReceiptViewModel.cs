using KerashineERP.Models.Production;

namespace KerashineERP.Models.ViewModels
{
    public class ProductionReceiptViewModel
    {
        public PRO_ProductionReceiptHeader Header { get; set; } = new();
        public List<PRO_ProductionOrderHeader> ProductionOrders { get; set; } = new();
    }
}