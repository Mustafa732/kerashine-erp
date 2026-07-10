using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Master
{
    [Table("SET_Company")]
    public class SET_Company : BaseEntity
    {
        [Key]
        public int CompanyID { get; set; }

        [Required]
        [Display(Name = "Business")]
        public int BusinessID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty; // Kerashine Pvt Ltd, Kerashine Trading

        [StringLength(20)]
        [Display(Name = "Company Code")]
        public string? CompanyCode { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(20)]
        [Display(Name = "NTN No")]
        public string? NTNNo { get; set; }

        [StringLength(20)]
        [Display(Name = "STRN No")]
        public string? STRNNo { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey("BusinessID")]
        public virtual SET_Business? Business { get; set; }
    }
}