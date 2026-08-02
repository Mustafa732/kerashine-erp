using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerashineERP.Models.Purchase
{
    [Table("AP_PurchaseOrderDetail")]
    public class AP_PurchaseOrderDetail
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int POHeaderID { get; set; }

        [Key, Column(Order = 2)]
        public int PODetailID { get; set; }

        public int SerialNo { get; set; }

        [Required]
        public int ItemID { get; set; }

        public int? UOMID { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string? Remarks { get; set; }

        public bool Status { get; set; } = true;

        // Base Identity
        public Guid CreatedBy { get; set; }
        public string CreatedByValue { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }
        public string? UpdatedByValue { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("CompanyID, POHeaderID")]
        public virtual AP_PurchaseOrderHeader? Header { get; set; }
    }
}