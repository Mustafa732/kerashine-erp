using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    [Table("PRO_ProductionOrderDetail")]
    public class PRO_ProductionOrderDetail : BaseEntity
    {
        [Key, Column(Order = 0)]
        public int CompanyID { get; set; }

        [Key, Column(Order = 1)]
        public int ProductionOrderID { get; set; }

        [Key, Column(Order = 2)]
        public int ProductionOrderDetailID { get; set; }

        [Required]
        public int MaterialID { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RequiredQty { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal IssuedQty { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal WastagePercent { get; set; } = 0;

        public int? UOMId { get; set; }

        [StringLength(200)]
        public string? Remarks { get; set; }

        [ForeignKey("CompanyID, MaterialID")]
        public virtual INV_Item? Material { get; set; }

        [ForeignKey("CompanyID, UOMId")]
        public virtual INV_SET_UOM? UOM { get; set; }

        [ForeignKey("CompanyID, ProductionOrderID")]
        public virtual PRO_ProductionOrderHeader? Header { get; set; }
    }
}