using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    [Table("PRO_RecipeDetail")]
    public class PRO_RecipeDetail : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int RecipeID { get; set; }

        [Key, Column(Order = 2)]
        public int RecipeDetailID { get; set; }

        [Required]
        public int MaterialID { get; set; }

        [Required, Column(TypeName = "decimal(18,4)")]
        public decimal QuantityRequired { get; set; }

        [Required]
        public int UOMId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal WastagePercent { get; set; } = 0;

        [StringLength(200)]
        public string? Remarks { get; set; }

        [ForeignKey("CompanyID, RecipeID")]
        public virtual PRO_RecipeHeader? RecipeHeader { get; set; }

        [ForeignKey("CompanyID, MaterialID")]
        public virtual INV_Item? Material { get; set; }

        [ForeignKey("CompanyID, UOMId")]
        public virtual INV_SET_UOM? UOM { get; set; }
    }
}