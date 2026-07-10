using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Master;

namespace KerashineERP.Models.Inventory
{
    public class INV_StockTransactionHeader : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public long StockTransactionID { get; set; }

        [Required]
        [Display(Name = "Document No")]
        public int DocumentNo { get; set; } // Auto increment per year/type

        [Required]
        [Display(Name = "Document Date")]
        [Column(TypeName = "date")]
        public DateTime DocumentDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Transaction Type")]
        public int DocumentTypeID { get; set; } // FK to DocumentType Master

        [Display(Name = "From Location")]
        public int? FromLocationID { get; set; }

        [Display(Name = "To Location")]
        public int? ToLocationID { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? TotalAmount { get; set; }

        [Required]
        [Display(Name = "Financial Year")]
        public int FiscalID { get; set; } // FK to Fiscal Year Master

        [Required]
        public short StatusCode { get; set; }

        // Navigation Properties
        public virtual ICollection<INV_StockTransactionDetail> StockTransactionDetail { get; set; } = new List<INV_StockTransactionDetail>();

        [ValidateNever]
        [ForeignKey("CompanyID, DocumentTypeID")]
        public virtual SET_DocumentType FK_INV_StockTransactionHeader_SET_DocumentType { get; set; } = null!;

        [ValidateNever]
        [ForeignKey("CompanyID, FromLocationID")]
        public virtual SET_Location FK_INV_StockTransactionHeader_SET_Location_From { get; set; } = null!;

        [ValidateNever]
        [ForeignKey("CompanyID, ToLocationID")]
        public virtual SET_Location FK_INV_StockTransactionHeader_SET_Location_To { get; set; } = null!;

        [ValidateNever]
        [ForeignKey("CompanyID, FiscalID")]
        public virtual SET_Fiscal FK_INV_StockTransactionHeader_SET_Fiscal { get; set; } = null!;
    }
}