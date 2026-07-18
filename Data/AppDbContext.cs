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
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

        public DbSet<PRO_RecipeHeader> PRO_RecipeHeader { get; set; }
        public DbSet<PRO_RecipeDetail> PRO_RecipeDetail { get; set; }

        // --- YE 2 ADD KIYE ---
        public DbSet<PRO_ProductionOrderHeader> PRO_ProductionOrderHeader { get; set; }
        public DbSet<PRO_ProductionOrderDetail> PRO_ProductionOrderDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SET_Business>().HasKey(b => b.BusinessID);
            modelBuilder.Entity<SET_Company>().HasKey(c => c.CompanyID);
            modelBuilder.Entity<SET_Company>().HasOne(c => c.Business).WithMany(b => b.Companies).HasForeignKey(c => c.BusinessID).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SET_DocumentType>().HasKey(d => new { d.CompanyID, d.DocumentTypeID });
            modelBuilder.Entity<SET_Location>().HasKey(l => new { l.CompanyID, l.LocationID });
            modelBuilder.Entity<SET_Fiscal>().HasKey(f => new { f.CompanyID, f.FiscalID });
            modelBuilder.Entity<INV_StockTransactionHeader>().HasKey(h => new { h.CompanyID, h.StockTransactionID });
            modelBuilder.Entity<INV_StockTransactionHeader>().HasOne(h => h.FK_INV_StockTransactionHeader_SET_DocumentType).WithMany().HasForeignKey(h => new { h.CompanyID, h.DocumentTypeID }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<INV_StockTransactionHeader>().HasOne(h => h.FK_INV_StockTransactionHeader_SET_Location_From).WithMany().HasForeignKey(h => new { h.CompanyID, h.FromLocationID }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<INV_StockTransactionHeader>().HasOne(h => h.FK_INV_StockTransactionHeader_SET_Location_To).WithMany().HasForeignKey(h => new { h.CompanyID, h.ToLocationID }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<INV_StockTransactionHeader>().HasOne(h => h.FK_INV_StockTransactionHeader_SET_Fiscal).WithMany().HasForeignKey(h => new { h.CompanyID, h.FiscalID }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<INV_StockTransactionDetail>().HasKey(d => new { d.CompanyID, d.StockTransactionID, d.TransactionDetailID });
            modelBuilder.Entity<INV_StockTransactionDetail>().HasOne(d => d.Header).WithMany(h => h.StockTransactionDetail).HasForeignKey(d => new { d.CompanyID, d.StockTransactionID }).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<INV_StockTransactionDetail>().HasOne(d => d.Item).WithMany().HasForeignKey(d => new { d.CompanyID, d.ItemID }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<INV_SET_Category>().HasKey(c => new { c.CompanyID, c.CategoryId });
            modelBuilder.Entity<INV_SET_UOM>().HasKey(u => new { u.CompanyID, u.UOMId });
            modelBuilder.Entity<INV_Item>().HasKey(i => new { i.CompanyID, i.ItemId });
            modelBuilder.Entity<INV_Item>().HasOne(i => i.Category).WithMany().HasForeignKey(i => new { i.CompanyID, i.CategoryId }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<INV_Item>().HasOne(i => i.UOM).WithMany().HasForeignKey(i => new { i.CompanyID, i.UOMId }).OnDelete(DeleteBehavior.Restrict);
               
            modelBuilder.Entity<PRO_RecipeHeader>().HasKey(x => new { x.CompanyID, x.RecipeID });
            modelBuilder.Entity<PRO_RecipeDetail>().HasKey(x => new { x.CompanyID, x.RecipeID, x.RecipeDetailID });
            modelBuilder.Entity<PRO_RecipeHeader>().HasOne(x => x.Product).WithMany().HasForeignKey(x => new { x.CompanyID, x.ProductID }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PRO_RecipeDetail>().HasOne(x => x.RecipeHeader).WithMany(x => x.RecipeDetails).HasForeignKey(x => new { x.CompanyID, x.RecipeID }).OnDelete(DeleteBehavior.Cascade);

            // --- YE MAPPING ADD KI ---
            modelBuilder.Entity<PRO_ProductionOrderHeader>().HasKey(x => new { x.CompanyID, x.ProductionOrderID });
            modelBuilder.Entity<PRO_ProductionOrderDetail>().HasKey(x => new { x.CompanyID, x.ProductionOrderID, x.ProductionOrderDetailID });

            modelBuilder.Entity<PRO_ProductionOrderHeader>()
               .HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => new { x.CompanyID, x.ProductID })
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PRO_ProductionOrderHeader>()
               .HasOne(x => x.Recipe)
               .WithMany()
               .HasForeignKey(x => new { x.CompanyID, x.RecipeID })
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PRO_ProductionOrderDetail>()
               .HasOne(x => x.Header)
               .WithMany(x => x.OrderDetails)
               .HasForeignKey(x => new { x.CompanyID, x.ProductionOrderID })
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}