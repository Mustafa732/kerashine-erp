using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Master
{
    public class SET_Fiscal : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int FiscalID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Fiscal Year")]
        public string FiscalYear { get; set; } = string.Empty; // 2025-2026

        [Required]
        [Display(Name = "Start Date")]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Is Current")]
        public bool IsCurrent { get; set; } = false;

        [Required]
        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; } = false; // Year close hone ke baad edit nahi hoga

        [Required]
        public bool IsActive { get; set; } = true;
    }
}