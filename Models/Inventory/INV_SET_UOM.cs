using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Inventory
{
    [Table("INV_SET_UOM")]
    public class INV_SET_UOM : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int UOMId { get; set; }

        [Required(ErrorMessage = "UOM Name is required")]
        [StringLength(50)]
        [Display(Name = "UOM Name")]
        public string UOMName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UOM Code is required")]
        [StringLength(10)]
        [Display(Name = "UOM Code")]
        public string UOMCode { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}