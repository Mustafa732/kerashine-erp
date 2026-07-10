using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;

namespace KerashineERP.Models.Inventory
{
    [Table("INV_Item")]
    public class INV_Item : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Item Code is required")]
        [StringLength(20)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; } = string.Empty; // RM001, PM001, P001

        [Required(ErrorMessage = "Item Name is required")]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty; // Shampoo, SLES, Bottle

        [Required(ErrorMessage = "Item Type is required")]
        [StringLength(5)]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; } = string.Empty; // RM, PM, FG

        [StringLength(50)]
        [Display(Name = "Size")]
        public string? ItemSize { get; set; } // 250, 500, 300

        [StringLength(10)]
        [Display(Name = "Size UOM")]
        public string? SizeUOM { get; set; } // ml, g, L

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual INV_SET_Category? Category { get; set; }

        [Required]
        [Display(Name = "Stock UOM")]
        public int UOMId { get; set; } // LTR, KG, PCS

        [ForeignKey("UOMId")]
        public virtual INV_SET_UOM? UOM { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Reorder Level")]
        public decimal ReorderLevel { get; set; } = 0;

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Min Stock")]
        public decimal MinStock { get; set; } = 0;

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Max Stock")]
        public decimal MaxStock { get; set; } = 0;

        [StringLength(50)]
        [Display(Name = "HS Code")]
        public string? HsCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Barcode")]
        public string? Barcode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? ItemImage { get; set; }
    }
}