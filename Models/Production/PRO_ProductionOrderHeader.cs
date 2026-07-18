using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    [Table("PRO_ProductionOrderHeader")]
    public class PRO_ProductionOrderHeader : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int ProductionOrderID { get; set; }

        [StringLength(40)]
        public string ProductionOrderNo { get; set; } = string.Empty;

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int RecipeID { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PlannedQty { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BatchSize { get; set; }

        [StringLength(20)]
        public string BatchSizeUOM { get; set; } = "KG";

        public DateTime PlannedDate { get; set; } = DateTime.Now;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public short StatusCode { get; set; } = 5; // TransactionStatus

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Remarks { get; set; }

        [ForeignKey("CompanyID, ProductID")]
        public virtual INV_Item? Product { get; set; }

        [ForeignKey("CompanyID, RecipeID")]
        public virtual PRO_RecipeHeader? Recipe { get; set; }

        public virtual ICollection<PRO_ProductionOrderDetail> OrderDetails { get; set; } = new List<PRO_ProductionOrderDetail>();
    }
}