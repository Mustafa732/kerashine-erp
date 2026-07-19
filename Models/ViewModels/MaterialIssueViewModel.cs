using KerashineERP.Models.Production;

namespace KerashineERP.Models.ViewModels
{
    public class MaterialIssueViewModel
    {
        public PRO_MaterialIssueHeader Header { get; set; } = new();
        public List<PRO_MaterialIssueDetail> Details { get; set; } = new();
        public List<PRO_ProductionOrderHeader> ProductionOrders { get; set; } = new();
    }
}