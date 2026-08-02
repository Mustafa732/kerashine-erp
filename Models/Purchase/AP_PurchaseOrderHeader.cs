using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerashineERP.Models.Purchase
{
    public enum POClosedCode : short
    {
        Open = 0,
        PartiallyReceived = 1,
        ClosedForReceiving = 2,
        Cancelled = 3
    }

    public enum POStatusCode : short
    {
        InProcess = 0,
        Approved = 1,
        Rejected = 2
    }

    [Table("AP_PurchaseOrderHeader")]
    public class AP_PurchaseOrderHeader
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int POHeaderID { get; set; }

        [Required]
        public int DocumentTypeID { get; set; }

        public int DocumentNumber { get; set; }

        [Required]
        public DateTime PODate { get; set; } = DateTime.Now;

        [Required]
        public int VendorID { get; set; }

        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }

        public short ClosedCode { get; set; } = 0; // POClosedCode Enum

        public short StatusCode { get; set; } = 0; // POStatusCode Enum - InProcess/Approved

        public bool Status { get; set; } = true;

        // Base Identity Columns
        public Guid CreatedBy { get; set; }
        public string CreatedByValue { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }
        public string? UpdatedByValue { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigations
        [ForeignKey("CompanyID, VendorID")]
        public virtual AR_SET_Customer? Vendor { get; set; }

        public virtual ICollection<AP_PurchaseOrderDetail> Details { get; set; } = new List<AP_PurchaseOrderDetail>();
    }
}