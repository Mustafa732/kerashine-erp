using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerashineERP.Models.Purchase
{
    public class AR_SET_BusinessPartnerTypes
    {
        [Key]
        public int CompanyID { get; set; }
        public int BusinessPartnerID { get; set; } // PUR_BusinessPartner.Id
        public short TypeCode { get; set; } // 1=Vendor, 2=Customer (SET_BusinessPartnerType se)

        [ForeignKey("BusinessPartnerID")]
        public virtual AR_SET_BusinessPartnerType? BusinessPartnerType { get; set; }
    }
}