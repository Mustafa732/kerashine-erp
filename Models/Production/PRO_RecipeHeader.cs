using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    [Table("PRO_RecipeHeader")]
    public class PRO_RecipeHeader : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int RecipeID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required, StringLength(40)]
        public string RecipeCode { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string RecipeName { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(18,4)")]
        public decimal BatchSize { get; set; }

        [Required, StringLength(20)]
        public string BatchSizeUOM { get; set; } = "KG";

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Remarks { get; set; }

        [ForeignKey("CompanyID, ProductID")]
        public virtual INV_Item? Product { get; set; }

        public virtual ICollection<PRO_RecipeDetail> RecipeDetails { get; set; } = new List<PRO_RecipeDetail>();
    }
}