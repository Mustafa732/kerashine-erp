using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Production
{
    [Table("PRO_MaterialIssueHeader")]
    public class PRO_MaterialIssueHeader : BaseEntity
    {
        [Column(Order = 0)]
        public int CompanyID { get; set; }

        [Column(Order = 1)]
        public int IssueID { get; set; }

        public string? IssueNo { get; set; } // MI-0001
        public int ProductionOrderID { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public int StatusCode { get; set; } = 5; // 5=InProcess, 1=Approved
        public string? Remarks { get; set; }

        // Navigation
        public PRO_ProductionOrderHeader? ProductionOrder { get; set; }
        public ICollection<PRO_MaterialIssueDetail>? IssueDetails { get; set; }
    }
}