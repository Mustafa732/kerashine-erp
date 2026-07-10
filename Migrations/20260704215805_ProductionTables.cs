using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerashineERP.Migrations
{
    /// <inheritdoc />
    public partial class ProductionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "SET_DocumentType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PRO_ProductionHeader",
                columns: table => new
                {
                    ProductionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    FiscalID = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    QuantityProduced = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostPerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CPR_250ml = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CPR_500ml = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<short>(type: "smallint", nullable: false),
                    ProductCompanyID = table.Column<int>(type: "int", nullable: true),
                    ProductItemId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PRO_ProductionHeader", x => x.ProductionID);
                    table.ForeignKey(
                        name: "FK_PRO_ProductionHeader_INV_SET_Item_ProductCompanyID_ProductItemId",
                        columns: x => new { x.ProductCompanyID, x.ProductItemId },
                        principalTable: "INV_SET_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" });
                });

            migrationBuilder.CreateTable(
                name: "PRO_ProductionDetail",
                columns: table => new
                {
                    ProductionDetailID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    MaterialType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialID = table.Column<int>(type: "int", nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostPerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialCompanyID = table.Column<int>(type: "int", nullable: true),
                    MaterialItemId = table.Column<int>(type: "int", nullable: true),
                    PRO_ProductionHeaderProductionID = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_PRO_ProductionDetail", x => x.ProductionDetailID);
                    table.ForeignKey(
                        name: "FK_PRO_ProductionDetail_INV_SET_Item_MaterialCompanyID_MaterialItemId",
                        columns: x => new { x.MaterialCompanyID, x.MaterialItemId },
                        principalTable: "INV_SET_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" });
                    table.ForeignKey(
                        name: "FK_PRO_ProductionDetail_PRO_ProductionHeader_PRO_ProductionHeaderProductionID",
                        column: x => x.PRO_ProductionHeaderProductionID,
                        principalTable: "PRO_ProductionHeader",
                        principalColumn: "ProductionID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionDetail_MaterialCompanyID_MaterialItemId",
                table: "PRO_ProductionDetail",
                columns: new[] { "MaterialCompanyID", "MaterialItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionDetail_PRO_ProductionHeaderProductionID",
                table: "PRO_ProductionDetail",
                column: "PRO_ProductionHeaderProductionID");

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionHeader_ProductCompanyID_ProductItemId",
                table: "PRO_ProductionHeader",
                columns: new[] { "ProductCompanyID", "ProductItemId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRO_ProductionDetail");

            migrationBuilder.DropTable(
                name: "PRO_ProductionHeader");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "SET_DocumentType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
