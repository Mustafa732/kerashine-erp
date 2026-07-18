using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerashineERP.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Full : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "INV_SET_Category",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INV_SET_Category", x => new { x.CompanyID, x.CategoryId });
                });

            migrationBuilder.CreateTable(
                name: "INV_SET_UOM",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    UOMId = table.Column<int>(type: "int", nullable: false),
                    UOMName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UOMCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INV_SET_UOM", x => new { x.CompanyID, x.UOMId });
                });

            migrationBuilder.CreateTable(
                name: "SET_Business",
                columns: table => new
                {
                    BusinessID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusinessCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_Business", x => x.BusinessID);
                });

            migrationBuilder.CreateTable(
                name: "SET_DocumentType",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_DocumentType", x => new { x.CompanyID, x.DocumentTypeID });
                });

            migrationBuilder.CreateTable(
                name: "SET_Fiscal",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    FiscalID = table.Column<int>(type: "int", nullable: false),
                    FiscalYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_Fiscal", x => new { x.CompanyID, x.FiscalID });
                });

            migrationBuilder.CreateTable(
                name: "SET_Location",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    LocationID = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_Location", x => new { x.CompanyID, x.LocationID });
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INV_Item",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ItemSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SizeUOM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    UOMId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReorderLevel = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinStock = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaxStock = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HsCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ItemImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INV_Item", x => new { x.CompanyID, x.ItemId });
                    table.ForeignKey(
                        name: "FK_INV_Item_INV_SET_Category_CompanyID_CategoryId",
                        columns: x => new { x.CompanyID, x.CategoryId },
                        principalTable: "INV_SET_Category",
                        principalColumns: new[] { "CompanyID", "CategoryId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INV_Item_INV_SET_UOM_CompanyID_UOMId",
                        columns: x => new { x.CompanyID, x.UOMId },
                        principalTable: "INV_SET_UOM",
                        principalColumns: new[] { "CompanyID", "UOMId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SET_Company",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessID = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NTNNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    STRNNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_Company", x => x.CompanyID);
                    table.ForeignKey(
                        name: "FK_SET_Company_SET_Business_BusinessID",
                        column: x => x.BusinessID,
                        principalTable: "SET_Business",
                        principalColumn: "BusinessID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INV_StockTransactionHeader",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    StockTransactionID = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNo = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "date", nullable: false),
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false),
                    FromLocationID = table.Column<int>(type: "int", nullable: true),
                    ToLocationID = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    FiscalID = table.Column<int>(type: "int", nullable: false),
                    StatusCode = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INV_StockTransactionHeader", x => new { x.CompanyID, x.StockTransactionID });
                    table.ForeignKey(
                        name: "FK_INV_StockTransactionHeader_SET_DocumentType_CompanyID_DocumentTypeID",
                        columns: x => new { x.CompanyID, x.DocumentTypeID },
                        principalTable: "SET_DocumentType",
                        principalColumns: new[] { "CompanyID", "DocumentTypeID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INV_StockTransactionHeader_SET_Fiscal_CompanyID_FiscalID",
                        columns: x => new { x.CompanyID, x.FiscalID },
                        principalTable: "SET_Fiscal",
                        principalColumns: new[] { "CompanyID", "FiscalID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INV_StockTransactionHeader_SET_Location_CompanyID_FromLocationID",
                        columns: x => new { x.CompanyID, x.FromLocationID },
                        principalTable: "SET_Location",
                        principalColumns: new[] { "CompanyID", "LocationID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INV_StockTransactionHeader_SET_Location_CompanyID_ToLocationID",
                        columns: x => new { x.CompanyID, x.ToLocationID },
                        principalTable: "SET_Location",
                        principalColumns: new[] { "CompanyID", "LocationID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PRO_RecipeHeader",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    RecipeID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    RecipeCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RecipeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BatchSize = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BatchSizeUOM = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRO_RecipeHeader", x => new { x.CompanyID, x.RecipeID });
                    table.ForeignKey(
                        name: "FK_PRO_RecipeHeader_INV_Item_CompanyID_ProductID",
                        columns: x => new { x.CompanyID, x.ProductID },
                        principalTable: "INV_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INV_StockTransactionDetail",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    StockTransactionID = table.Column<long>(type: "bigint", nullable: false),
                    TransactionDetailID = table.Column<long>(type: "bigint", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INV_StockTransactionDetail", x => new { x.CompanyID, x.StockTransactionID, x.TransactionDetailID });
                    table.ForeignKey(
                        name: "FK_INV_StockTransactionDetail_INV_Item_CompanyID_ItemID",
                        columns: x => new { x.CompanyID, x.ItemID },
                        principalTable: "INV_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INV_StockTransactionDetail_INV_StockTransactionHeader_CompanyID_StockTransactionID",
                        columns: x => new { x.CompanyID, x.StockTransactionID },
                        principalTable: "INV_StockTransactionHeader",
                        principalColumns: new[] { "CompanyID", "StockTransactionID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRO_RecipeDetail",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    RecipeID = table.Column<int>(type: "int", nullable: false),
                    RecipeDetailID = table.Column<int>(type: "int", nullable: false),
                    MaterialID = table.Column<int>(type: "int", nullable: false),
                    QuantityRequired = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UOMId = table.Column<int>(type: "int", nullable: false),
                    WastagePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRO_RecipeDetail", x => new { x.CompanyID, x.RecipeID, x.RecipeDetailID });
                    table.ForeignKey(
                        name: "FK_PRO_RecipeDetail_INV_Item_CompanyID_MaterialID",
                        columns: x => new { x.CompanyID, x.MaterialID },
                        principalTable: "INV_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PRO_RecipeDetail_INV_SET_UOM_CompanyID_UOMId",
                        columns: x => new { x.CompanyID, x.UOMId },
                        principalTable: "INV_SET_UOM",
                        principalColumns: new[] { "CompanyID", "UOMId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PRO_RecipeDetail_PRO_RecipeHeader_CompanyID_RecipeID",
                        columns: x => new { x.CompanyID, x.RecipeID },
                        principalTable: "PRO_RecipeHeader",
                        principalColumns: new[] { "CompanyID", "RecipeID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_INV_Item_CompanyID_CategoryId",
                table: "INV_Item",
                columns: new[] { "CompanyID", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_INV_Item_CompanyID_UOMId",
                table: "INV_Item",
                columns: new[] { "CompanyID", "UOMId" });

            migrationBuilder.CreateIndex(
                name: "IX_INV_StockTransactionDetail_CompanyID_ItemID",
                table: "INV_StockTransactionDetail",
                columns: new[] { "CompanyID", "ItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_INV_StockTransactionHeader_CompanyID_DocumentTypeID",
                table: "INV_StockTransactionHeader",
                columns: new[] { "CompanyID", "DocumentTypeID" });

            migrationBuilder.CreateIndex(
                name: "IX_INV_StockTransactionHeader_CompanyID_FiscalID",
                table: "INV_StockTransactionHeader",
                columns: new[] { "CompanyID", "FiscalID" });

            migrationBuilder.CreateIndex(
                name: "IX_INV_StockTransactionHeader_CompanyID_FromLocationID",
                table: "INV_StockTransactionHeader",
                columns: new[] { "CompanyID", "FromLocationID" });

            migrationBuilder.CreateIndex(
                name: "IX_INV_StockTransactionHeader_CompanyID_ToLocationID",
                table: "INV_StockTransactionHeader",
                columns: new[] { "CompanyID", "ToLocationID" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_RecipeDetail_CompanyID_MaterialID",
                table: "PRO_RecipeDetail",
                columns: new[] { "CompanyID", "MaterialID" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_RecipeDetail_CompanyID_UOMId",
                table: "PRO_RecipeDetail",
                columns: new[] { "CompanyID", "UOMId" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_RecipeHeader_CompanyID_ProductID",
                table: "PRO_RecipeHeader",
                columns: new[] { "CompanyID", "ProductID" });

            migrationBuilder.CreateIndex(
                name: "IX_SET_Company_BusinessID",
                table: "SET_Company",
                column: "BusinessID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "INV_StockTransactionDetail");

            migrationBuilder.DropTable(
                name: "PRO_RecipeDetail");

            migrationBuilder.DropTable(
                name: "SET_Company");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "INV_StockTransactionHeader");

            migrationBuilder.DropTable(
                name: "PRO_RecipeHeader");

            migrationBuilder.DropTable(
                name: "SET_Business");

            migrationBuilder.DropTable(
                name: "SET_DocumentType");

            migrationBuilder.DropTable(
                name: "SET_Fiscal");

            migrationBuilder.DropTable(
                name: "SET_Location");

            migrationBuilder.DropTable(
                name: "INV_Item");

            migrationBuilder.DropTable(
                name: "INV_SET_Category");

            migrationBuilder.DropTable(
                name: "INV_SET_UOM");
        }
    }
}
