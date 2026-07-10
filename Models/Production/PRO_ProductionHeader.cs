using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using KerashineERP.Models.Common;
using KerashineERP.Models.Inventory;

namespace KerashineERP.Models.Production
{
    public class PRO_ProductionHeader : BaseEntity
    {
        [Key]
        public long ProductionID { get; set; }
        public int CompanyID { get; set; }
        public int FiscalID { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public DateTime ProductionDate { get; set; }
        public int ProductID { get; set; }
        public decimal QuantityProduced { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal CPR_250ml { get; set; }
        public decimal CPR_500ml { get; set; }
        public string? Remarks { get; set; }
        public short StatusCode { get; set; }

        [ValidateNever]
        public virtual INV_Item? Product { get; set; } // ✅ Fix 5: Nullable

        [ValidateNever]
        public virtual ICollection<PRO_ProductionDetail>? ProductionDetails { get; set; } // ✅ Fix 6: Nullable
    }
}