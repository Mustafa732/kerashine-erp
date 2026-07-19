using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerashineERP.Migrations
{
    /// <inheritdoc />
    public partial class PRO_MaterialIssuanceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRO_MaterialIssueHeader",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    IssueID = table.Column<int>(type: "int", nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionOrderID = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PRO_MaterialIssueHeader", x => new { x.CompanyID, x.IssueID });
                    table.ForeignKey(
                        name: "FK_PRO_MaterialIssueHeader_PRO_ProductionOrderHeader_CompanyID_ProductionOrderID",
                        columns: x => new { x.CompanyID, x.ProductionOrderID },
                        principalTable: "PRO_ProductionOrderHeader",
                        principalColumns: new[] { "CompanyID", "ProductionOrderID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PRO_MaterialIssueDetail",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    IssueID = table.Column<int>(type: "int", nullable: false),
                    IssueDetailID = table.Column<int>(type: "int", nullable: false),
                    MaterialID = table.Column<int>(type: "int", nullable: false),
                    RequiredQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssuedQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UOMId = table.Column<int>(type: "int", nullable: false),
                    WastagePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaterialCompanyID = table.Column<int>(type: "int", nullable: true),
                    MaterialItemId = table.Column<int>(type: "int", nullable: true),
                    UOMCompanyID = table.Column<int>(type: "int", nullable: true),
                    UOMId1 = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PRO_MaterialIssueDetail", x => new { x.CompanyID, x.IssueID, x.IssueDetailID });
                    table.ForeignKey(
                        name: "FK_PRO_MaterialIssueDetail_INV_Item_MaterialCompanyID_MaterialItemId",
                        columns: x => new { x.MaterialCompanyID, x.MaterialItemId },
                        principalTable: "INV_Item",
                        principalColumns: new[] { "CompanyID", "ItemId" });
                    table.ForeignKey(
                        name: "FK_PRO_MaterialIssueDetail_INV_SET_UOM_UOMCompanyID_UOMId1",
                        columns: x => new { x.UOMCompanyID, x.UOMId1 },
                        principalTable: "INV_SET_UOM",
                        principalColumns: new[] { "CompanyID", "UOMId" });
                    table.ForeignKey(
                        name: "FK_PRO_MaterialIssueDetail_PRO_MaterialIssueHeader_CompanyID_IssueID",
                        columns: x => new { x.CompanyID, x.IssueID },
                        principalTable: "PRO_MaterialIssueHeader",
                        principalColumns: new[] { "CompanyID", "IssueID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_MaterialIssueDetail_MaterialCompanyID_MaterialItemId",
                table: "PRO_MaterialIssueDetail",
                columns: new[] { "MaterialCompanyID", "MaterialItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_MaterialIssueDetail_UOMCompanyID_UOMId1",
                table: "PRO_MaterialIssueDetail",
                columns: new[] { "UOMCompanyID", "UOMId1" });

            migrationBuilder.CreateIndex(
                name: "IX_PRO_MaterialIssueHeader_CompanyID_ProductionOrderID",
                table: "PRO_MaterialIssueHeader",
                columns: new[] { "CompanyID", "ProductionOrderID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRO_MaterialIssueDetail");

            migrationBuilder.DropTable(
                name: "PRO_MaterialIssueHeader");
        }
    }
}
