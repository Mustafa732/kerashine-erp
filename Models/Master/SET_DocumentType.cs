using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Master
{
    public class SET_DocumentType : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int DocumentTypeID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; } = string.Empty; // OPENING, GRN, ISSUE

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public string? TransactionType { get; set; } // 1 = IN, -1 = OUT, 0 = No Effect

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(10)]
        public string? Prefix { get; set; } // OP, GRN, MI
    }
}