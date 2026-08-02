using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Master;

namespace KerashineERP.Models.Inventory
{
    public class INV_StockTransactionDetail : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public long StockTransactionID { get; set; }

        [Key, Column(Order = 2)]
        public long TransactionDetailID { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemID { get; set; }

        [StringLength(50)]
        [Display(Name = "Batch No")]
        public string? BatchNo { get; set; }

        [Display(Name = "Expiry Date")]
        [Column(TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; } // +ve IN, -ve OUT

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string? Remarks { get; set; }

        // Add after Remarks property
        [Display(Name = "Source Header")]
        public int? SourceHeaderID { get; set; } // POHeaderID

        [Display(Name = "Source Detail")]
        public int? SourceDetailID { get; set; } // PODetailID

        [Display(Name = "Source Doc Type")]
        public int? SourceDocumentTypeID { get; set; }

        [Display(Name = "Source Code")]
        public int? SourceCode { get; set; } // PO-1001

        // Navigation Properties
        [ValidateNever]
        [ForeignKey("CompanyID, StockTransactionID")]
        public virtual INV_StockTransactionHeader? Header { get; set; }

        [ValidateNever]
        [ForeignKey("CompanyID, ItemID")]
        public virtual INV_Item? Item { get; set; }
    }
}