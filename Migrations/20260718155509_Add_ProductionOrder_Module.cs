using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerashineERP.Migrations
{
    /// <inheritdoc />
    public partial class Add_ProductionOrder_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRO_ProductionOrderHeader",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ProductionOrderID = table.Column<int>(type: "int", nullable: false),
                    ProductionOrderNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    RecipeID = table.Column<int>(type: "int", nullable: false),
                    PlannedQty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BatchSize = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BatchSizeUOM = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusCode = table.Column<short>(type: "smallint", nullable: false),
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
                    table.PrimaryKey("PK_PRO_ProductionOrderHeader", x => new { x.CompanyID, x.ProductionOrderID });
                    table.ForeignKey(
                        name: "FK_PRO_ProductionOrderHeader_INV_Item_CompanyID_ProductID",
                        columns: x => new { x.CompanyID, x.ProductID },
                        principalTable: "INV_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PRO_ProductionOrderHeader_PRO_RecipeHeader_CompanyID_RecipeID",
                        columns: x => new { x.CompanyID, x.RecipeID },
                        principalTable: "PRO_RecipeHeader",
                        principalColumns: new[] { "CompanyID", "RecipeID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PRO_ProductionOrderDetail",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ProductionOrderID = table.Column<int>(type: "int", nullable: false),
                    ProductionOrderDetailID = table.Column<int>(type: "int", nullable: false),
                    MaterialID = table.Column<int>(type: "int", nullable: false),
                    RequiredQty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IssuedQty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    WastagePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    UOMId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PRO_ProductionOrderDetail", x => new { x.CompanyID, x.ProductionOrderID, x.ProductionOrderDetailID });
                    table.ForeignKey(
                        name: "FK_PRO_ProductionOrderDetail_INV_Item_CompanyID_MaterialID",
                        columns: x => new { x.CompanyID, x.MaterialID },
                        principalTable: "INV_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PRO_ProductionOrderDetail_INV_SET_UOM_CompanyID_UOMId",
                        columns: x => new { x.CompanyID, x.UOMId },
                        principalTable: "INV_SET_UOM",
                        principalColumns: new[] { "CompanyID", "UOMId" });
                    table.ForeignKey(
                        name: "FK_PRO_ProductionOrderDetail_PRO_ProductionOrderHeader_CompanyID_ProductionOrderID",
                        columns: x => new { x.CompanyID, x.ProductionOrderID },
                        principalTable: "PRO_ProductionOrderHeader",
                        principalColumns: new[] { "CompanyID", "ProductionOrderID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionOrderDetail_CompanyID_MaterialID",
                table: "PRO_ProductionOrderDetail",
                columns: new[] { "CompanyID", "MaterialID" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionOrderDetail_CompanyID_UOMId",
                table: "PRO_ProductionOrderDetail",
                columns: new[] { "CompanyID", "UOMId" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionOrderHeader_CompanyID_ProductID",
                table: "PRO_ProductionOrderHeader",
                columns: new[] { "CompanyID", "ProductID" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionOrderHeader_CompanyID_RecipeID",
                table: "PRO_ProductionOrderHeader",
                columns: new[] { "CompanyID", "RecipeID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRO_ProductionOrderDetail");

            migrationBuilder.DropTable(
                name: "PRO_ProductionOrderHeader");
        }
    }
}
