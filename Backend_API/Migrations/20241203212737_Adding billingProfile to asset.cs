using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class AddingbillingProfiletoasset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingProfileId",
                table: "CustomerAssets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssets_BillingProfileId",
                table: "CustomerAssets",
                column: "BillingProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAssets_BillingProfiles_BillingProfileId",
                table: "CustomerAssets",
                column: "BillingProfileId",
                principalTable: "BillingProfiles",
                principalColumn: "BillingProfileId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAssets_BillingProfiles_BillingProfileId",
                table: "CustomerAssets");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAssets_BillingProfileId",
                table: "CustomerAssets");

            migrationBuilder.DropColumn(
                name: "BillingProfileId",
                table: "CustomerAssets");
        }
    }
}
