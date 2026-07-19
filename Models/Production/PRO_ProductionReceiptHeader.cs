using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Production
{
    [Table("PRO_ProductionReceiptHeader")]
    public class PRO_ProductionReceiptHeader : BaseEntity
    {
        public int CompanyID { get; set; }
        public int ReceiptID { get; set; }
        public string ReceiptNo { get; set; } = "";
        public int ProductionOrderID { get; set; }
        public DateTime ReceiptDate { get; set; } = DateTime.Now;
        public decimal ReceivedQty { get; set; }
        public int StatusCode { get; set; } = 5; // 5=InProcess, 1=Approved
        public string Remarks { get; set; } = "";

        public PRO_ProductionOrderHeader ?ProductionOrder { get; set; }
    }
}