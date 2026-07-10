using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KerashineERP.Models.Common
{
    public abstract class BaseEntity
    {
        [Column("Status")]
        public bool Status { get; set; } = true; // 1 = Active/Not Deleted, 0 = Deleted

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("CreatedBy")]
        public Guid CreatedBy { get; set; } // UserId

        [Column("CreatedByValue")]
        [StringLength(256)]
        public string? CreatedByValue { get; set; } // Username

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }

        [Column("UpdatedBy")]
        public Guid? UpdatedBy { get; set; } // UserId

        [Column("UpdatedByValue")]
        [StringLength(256)]
        public string? UpdatedByValue { get; set; } // Username
    }
}