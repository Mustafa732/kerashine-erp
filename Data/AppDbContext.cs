using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.Master;
using KerashineERP.Models.Production;

namespace KerashineERP.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Inventory Masters
        public DbSet<INV_SET_Category> INV_SET_Category { get; set; }
        public DbSet<INV_SET_UOM> INV_SET_UOM { get; set; }
        public DbSet<INV_Item> INV_Item { get; set; }
        public DbSet<SET_Location> SET_Location { get; set; }

        // Settings Masters
        public DbSet<SET_Business> SET_Business { get; set; }
        public DbSet<SET_Company> SET_Company { get; set; }
        public DbSet<SET_DocumentType> SET_DocumentType { get; set; }
        public DbSet<SET_Fiscal> SET_Fiscal { get; set; }

        // Stock Transactions
        public DbSet<INV_StockTransactionHeader> INV_StockTransactionHeader { get; set; }
        public DbSet<INV_StockTransactionDetail> INV_StockTransactionDetail { get; set; }

        public DbSet<PRO_ProductionHeader> PRO_ProductionHeader { get; set; }
        public DbSet<PRO_ProductionDetail> PRO_ProductionDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === SET_Business Config ===
            modelBuilder.Entity<SET_Business>()
               .HasKey(b => b.BusinessID);

            // === SET_Company Config ===
            modelBuilder.Entity<SET_Company>()
               .HasKey(c => c.CompanyID);

            modelBuilder.Entity<SET_Company>()
               .HasOne(c => c.Business)
               .WithMany(b => b.Companies)
               .HasForeignKey(c => c.BusinessID)
               .OnDelete(DeleteBehavior.Restrict);

            // === SET_DocumentType Config ===
            modelBuilder.Entity<SET_DocumentType>()
               .HasKey(d => new { d.CompanyID, d.DocumentTypeID });

            // === SET_Location Config ===
            modelBuilder.Entity<SET_Location>()
               .HasKey(l => new { l.CompanyID, l.LocationID });

            // === SET_Fiscal Config ===
            modelBuilder.Entity<SET_Fiscal>()
               .HasKey(f => new { f.CompanyID, f.FiscalID });

            // === INV_StockTransactionHeader Config ===
            modelBuilder.Entity<INV_StockTransactionHeader>()
               .HasKey(h => new { h.CompanyID, h.StockTransactionID });

            // Header -> DocumentType Relation
            modelBuilder.Entity<INV_StockTransactionHeader>()
               .HasOne(h => h.FK_INV_StockTransactionHeader_SET_DocumentType)
               .WithMany()
               .HasForeignKey(h => new { h.CompanyID, h.DocumentTypeID })
               .OnDelete(DeleteBehavior.Restrict);

            // Header -> FromLocation Relation - FIXED
            modelBuilder.Entity<INV_StockTransactionHeader>()
               .HasOne(h => h.FK_INV_StockTransactionHeader_SET_Location_From)
               .WithMany()
               .HasForeignKey(h => new { h.CompanyID, h.FromLocationID })
               .OnDelete(DeleteBehavior.Restrict);

            // Header -> ToLocation Relation - FIXED
            modelBuilder.Entity<INV_StockTransactionHeader>()
               .HasOne(h => h.FK_INV_StockTransactionHeader_SET_Location_To)
               .WithMany()
               .HasForeignKey(h => new { h.CompanyID, h.ToLocationID })
               .OnDelete(DeleteBehavior.Restrict);

            // Header -> FiscalYear Relation - FIXED
            modelBuilder.Entity<INV_StockTransactionHeader>()
               .HasOne(h => h.FK_INV_StockTransactionHeader_SET_Fiscal)
               .WithMany()
               .HasForeignKey(h => new { h.CompanyID, h.FiscalID })
               .OnDelete(DeleteBehavior.Restrict);

            // === INV_StockTransactionDetail Config ===
            modelBuilder.Entity<INV_StockTransactionDetail>()
               .HasKey(d => new { d.CompanyID, d.StockTransactionID, d.TransactionDetailID });

            // Detail -> Header Relation
            modelBuilder.Entity<INV_StockTransactionDetail>()
               .HasOne(d => d.Header)
               .WithMany(h => h.StockTransactionDetail)
               .HasForeignKey(d => new { d.CompanyID, d.StockTransactionID })
               .OnDelete(DeleteBehavior.Cascade);

            // Detail -> Item Relation - FIXED: CompanyID add kiya
            modelBuilder.Entity<INV_StockTransactionDetail>()
               .HasOne(d => d.Item)
               .WithMany()
               .HasForeignKey(d => new { d.CompanyID, d.ItemID })
               .OnDelete(DeleteBehavior.Restrict);

            // === INV_SET_ItemCategory Config ===
            modelBuilder.Entity<INV_SET_Category>()
               .HasKey(c => new { c.CompanyID, c.CategoryId });

            // === INV_SET_UOM Config ===
            modelBuilder.Entity<INV_SET_UOM>()
               .HasKey(u => new { u.CompanyID, u.UOMId });

            // === INV_SET_Item Config ===
            modelBuilder.Entity<INV_Item>()
               .HasKey(i => new { i.CompanyID, i.ItemId });

            modelBuilder.Entity<INV_Item>()
               .HasOne(i => i.Category)
               .WithMany()
               .HasForeignKey(i => new { i.CompanyID, i.CategoryId })
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<INV_Item>()
               .HasOne(i => i.UOM)
               .WithMany()
               .HasForeignKey(i => new { i.CompanyID, i.UOMId })
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}