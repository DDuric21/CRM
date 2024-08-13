using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class fixingbillingprofile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BillingProfiles",
                table: "BillingProfiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BillingProfiles");

            migrationBuilder.AddColumn<string>(
                name: "BillingProfileId",
                table: "BillingProfiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CustomerID",
                table: "BillingProfiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AddressID",
                table: "BillingProfiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BillingProfiles",
                table: "BillingProfiles",
                column: "BillingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingProfiles_AddressID",
                table: "BillingProfiles",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_BillingProfiles_CustomerID",
                table: "BillingProfiles",
                column: "CustomerID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BillingProfile_Key_Format",
                table: "BillingProfiles",
                sql: "BillingProfileId LIKE '[0-9]-%[0-9]'");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingProfiles_Addresses_AddressID",
                table: "BillingProfiles",
                column: "AddressID",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BillingProfiles_Customers_CustomerID",
                table: "BillingProfiles",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingProfiles_Addresses_AddressID",
                table: "BillingProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_BillingProfiles_Customers_CustomerID",
                table: "BillingProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BillingProfiles",
                table: "BillingProfiles");

            migrationBuilder.DropIndex(
                name: "IX_BillingProfiles_AddressID",
                table: "BillingProfiles");

            migrationBuilder.DropIndex(
                name: "IX_BillingProfiles_CustomerID",
                table: "BillingProfiles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BillingProfile_Key_Format",
                table: "BillingProfiles");

            migrationBuilder.DropColumn(
                name: "BillingProfileId",
                table: "BillingProfiles");

            migrationBuilder.DropColumn(
                name: "AddressID",
                table: "BillingProfiles");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "BillingProfiles");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BillingProfiles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "BillingProfiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BillingProfiles",
                table: "BillingProfiles",
                column: "Id");
        }
    }
}
