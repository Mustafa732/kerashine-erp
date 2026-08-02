using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerashineERP.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessPartnerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AR_SET_BusinessPartnerType",
                columns: table => new
                {
                    BusinessPartnerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessPartnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AR_SET_BusinessPartnerType", x => x.BusinessPartnerID);
                });

            migrationBuilder.CreateTable(
                name: "AR_SET_Customer",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CNICNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NTNNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AR_SET_Customer", x => new { x.CompanyID, x.CustomerID });
                });

            migrationBuilder.CreateTable(
                name: "AR_SET_BusinessPartnerTypes",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    BusinessPartnerID = table.Column<int>(type: "int", nullable: false),
                    TypeCode = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AR_SET_BusinessPartnerTypes", x => new { x.CompanyID, x.BusinessPartnerID, x.TypeCode });
                    table.ForeignKey(
                        name: "FK_AR_SET_BusinessPartnerTypes_AR_SET_BusinessPartnerType_BusinessPartnerID",
                        column: x => x.BusinessPartnerID,
                        principalTable: "AR_SET_BusinessPartnerType",
                        principalColumn: "BusinessPartnerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AR_SET_BusinessPartnerTypes_BusinessPartnerID",
                table: "AR_SET_BusinessPartnerTypes",
                column: "BusinessPartnerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AR_SET_BusinessPartnerTypes");

            migrationBuilder.DropTable(
                name: "AR_SET_Customer");

            migrationBuilder.DropTable(
                name: "AR_SET_BusinessPartnerType");
        }
    }
}
