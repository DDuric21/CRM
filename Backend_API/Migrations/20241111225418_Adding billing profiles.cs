using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class Addingbillingprofiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingProfiles",
                columns: table => new
                {
                    BillingProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    AddressID = table.Column<long>(type: "bigint", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingProfiles", x => x.BillingProfileId);
                    table.CheckConstraint("CK_BillingProfile_Key_Format", "BillingProfileId LIKE '[0-9]-%[0-9]'");
                    table.ForeignKey(
                        name: "FK_BillingProfiles_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BillingProfiles_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillingProfiles_AddressID",
                table: "BillingProfiles",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_BillingProfiles_CustomerID",
                table: "BillingProfiles",
                column: "CustomerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingProfiles");
        }
    }
}
