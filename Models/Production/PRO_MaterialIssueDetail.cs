using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    [Table("PRO_MaterialIssueDetail")]
    public class PRO_MaterialIssueDetail : BaseEntity
    {
        [Column(Order = 0)]
        public int CompanyID { get; set; }

        [Column(Order = 1)]
        public int IssueID { get; set; }

        [Column(Order = 2)]
        public int IssueDetailID { get; set; }

        public int MaterialID { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal IssuedQty { get; set; }
        public int UOMId { get; set; }
        public decimal WastagePercent { get; set; }

        // Navigation
        public INV_Item? Material { get; set; }
        public INV_SET_UOM? UOM { get; set; }

        [ForeignKey("CompanyID, IssueID")]
        public virtual PRO_MaterialIssueHeader? Header { get; set; }
    }
}