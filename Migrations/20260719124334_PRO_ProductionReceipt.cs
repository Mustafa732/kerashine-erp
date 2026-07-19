using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerashineERP.Migrations
{
    /// <inheritdoc />
    public partial class PRO_ProductionReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRO_ProductionReceiptHeader",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ReceiptID = table.Column<int>(type: "int", nullable: false),
                    ReceiptNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionOrderID = table.Column<int>(type: "int", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_PRO_ProductionReceiptHeader", x => new { x.CompanyID, x.ReceiptID });
                    table.ForeignKey(
                        name: "FK_PRO_ProductionReceiptHeader_PRO_ProductionOrderHeader_CompanyID_ProductionOrderID",
                        columns: x => new { x.CompanyID, x.ProductionOrderID },
                        principalTable: "PRO_ProductionOrderHeader",
                        principalColumns: new[] { "CompanyID", "ProductionOrderID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_ProductionReceiptHeader_CompanyID_ProductionOrderID",
                table: "PRO_ProductionReceiptHeader",
                columns: new[] { "CompanyID", "ProductionOrderID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRO_ProductionReceiptHeader");
        }
    }
}
