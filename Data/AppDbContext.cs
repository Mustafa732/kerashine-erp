using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KerashineERP.Models.Inventory;
using KerashineERP.Models.Master;
using KerashineERP.Models.Production;
using KerashineERP.Models.Purchase;

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

        public DbSet<PRO_MaterialIssueHeader> PRO_MaterialIssueHeader { get; set; }
        public DbSet<PRO_MaterialIssueDetail> PRO_MaterialIssueDetail { get; set; }

        public DbSet<PRO_ProductionReceiptHeader> PRO_ProductionReceiptHeader { get; set; }
        // Purchase
        public DbSet<AR_SET_BusinessPartnerType> AR_SET_BusinessPartnerType { get; set; }
        public DbSet<AR_SET_Customer> AR_SET_Customer { get; set; }
        public DbSet<AR_SET_BusinessPartnerTypes> AR_SET_BusinessPartnerTypes { get; set; }
        public DbSet<AP_PurchaseOrderHeader> AP_PurchaseOrderHeader { get; set; }
        public DbSet<AP_PurchaseOrderDetail> AP_PurchaseOrderDetail { get; set; }

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
               
            modelBuilder.Entity<PRO_MaterialIssueHeader>().HasKey(x => new { x.CompanyID, x.IssueID });
            modelBuilder.Entity<PRO_MaterialIssueDetail>().HasKey(x => new { x.CompanyID, x.IssueID, x.IssueDetailID });

            modelBuilder.Entity<PRO_MaterialIssueHeader>()
                .HasOne(x => x.ProductionOrder)
                .WithMany()
                .HasForeignKey(x => new { x.CompanyID, x.ProductionOrderID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PRO_MaterialIssueDetail>()
                .HasOne(x => x.Header)
                .WithMany(x => x.IssueDetails)
                .HasForeignKey(x => new { x.CompanyID, x.IssueID })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PRO_ProductionReceiptHeader>().HasKey(x => new { x.CompanyID, x.ReceiptID });
            
            modelBuilder.Entity<PRO_ProductionReceiptHeader>()
                .HasOne(x => x.ProductionOrder).WithMany().HasForeignKey(x => new { x.CompanyID, x.ProductionOrderID }).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AR_SET_BusinessPartnerType>().HasKey(x => x.BusinessPartnerID);

            modelBuilder.Entity<AR_SET_Customer>().HasKey(x => new { x.CompanyID, x.CustomerID });

            modelBuilder.Entity<AR_SET_BusinessPartnerTypes>().HasKey(x => new { x.CompanyID, x.BusinessPartnerID, x.TypeCode });

            modelBuilder.Entity<AP_PurchaseOrderHeader>()
                .HasKey(x => new { x.CompanyID, x.POHeaderID });

            modelBuilder.Entity<AP_PurchaseOrderDetail>()
               .HasKey(x => new { x.CompanyID, x.POHeaderID, x.PODetailID });
        }
    }
}