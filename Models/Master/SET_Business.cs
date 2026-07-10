using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Master
{
    [Table("SET_Business")]
    public class SET_Business : BaseEntity
    {
        [Key]
        public int BusinessID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty; // Kerashine, ABC Group

        [StringLength(20)]
        [Display(Name = "Business Code")]
        public string? BusinessCode { get; set; } // KSH, ABC

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<SET_Company> Companies { get; set; } = new List<SET_Company>();
    }
}