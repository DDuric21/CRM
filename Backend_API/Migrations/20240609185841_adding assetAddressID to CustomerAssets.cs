using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class addingassetAddressIDtoCustomerAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssetAddressID",
                table: "CustomerAssets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Options_AssetID",
                table: "Options",
                column: "AssetID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssets_AssetID",
                table: "CustomerAssets",
                column: "AssetID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssets_CustomerID",
                table: "CustomerAssets",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAssets_Assets_AssetID",
                table: "CustomerAssets",
                column: "AssetID",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAssets_Customers_CustomerID",
                table: "CustomerAssets",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Options_Assets_AssetID",
                table: "Options",
                column: "AssetID",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAssets_Assets_AssetID",
                table: "CustomerAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAssets_Customers_CustomerID",
                table: "CustomerAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_Options_Assets_AssetID",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Options_AssetID",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAssets_AssetID",
                table: "CustomerAssets");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAssets_CustomerID",
                table: "CustomerAssets");

            migrationBuilder.DropColumn(
                name: "AssetAddressID",
                table: "CustomerAssets");
        }
    }
}
