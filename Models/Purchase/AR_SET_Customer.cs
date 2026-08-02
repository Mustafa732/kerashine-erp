using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerashineERP.Models.Purchase
{
    public class AR_SET_Customer
    {
        [Key]
        public int CompanyID { get; set; } // FK
        public int CustomerID { get; set; } // Identity nahi, ye Type ID hoga

        // Main Partner ka record
        public int Id { get; set; } // PK Identity
        public string? CustomerCode { get; set; } // Auto: BP-0001

        [Required]
        public string? CustomerName { get; set; } // ABDUL MAJEED etc

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CNICNumber { get; set; }
        public string? NTNNumber { get; set; }
        public string? EmailAddress { get; set; }

        public int StatusCode { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }
}