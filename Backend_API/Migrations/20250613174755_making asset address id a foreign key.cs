using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class makingassetaddressidaforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssets_AssetAddressID",
                table: "CustomerAssets",
                column: "AssetAddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAssets_Addresses_AssetAddressID",
                table: "CustomerAssets",
                column: "AssetAddressID",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAssets_Addresses_AssetAddressID",
                table: "CustomerAssets");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAssets_AssetAddressID",
                table: "CustomerAssets");
        }
    }
}
