using System.ComponentModel.DataAnnotations;

namespace KerashineERP.Models.Purchase
{
    public class AR_SET_BusinessPartnerType
    {
        [Key]
        public int BusinessPartnerID { get; set; } // 1=Vendor, 2=Customer

        [Required]
        public string BusinessPartnerName { get; set; } = string.Empty;

        public string? ShipTo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}