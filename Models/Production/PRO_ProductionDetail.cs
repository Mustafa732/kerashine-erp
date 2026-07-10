using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    public class PRO_ProductionDetail : BaseEntity
    {
        [Key]
        public long ProductionDetailID { get; set; }
        public long ProductionID { get; set; }
        public int CompanyID { get; set; }
        public string? MaterialType { get; set; } // "Raw" ya "Packaging"
        public int MaterialID { get; set; }
        public decimal QuantityUsed { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
        public string? Remarks { get; set; }

        [ValidateNever]
        public virtual INV_Item? Material { get; set; }
    }
}