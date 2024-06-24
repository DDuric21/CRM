using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class orderCustomerAssetIDmakingnotrequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_CustomerAssets_CustomerAssetsID",
                table: "Order");

            migrationBuilder.AlterColumn<long>(
                name: "CustomerAssetsID",
                table: "Order",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_CustomerAssets_CustomerAssetsID",
                table: "Order",
                column: "CustomerAssetsID",
                principalTable: "CustomerAssets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_CustomerAssets_CustomerAssetsID",
                table: "Order");

            migrationBuilder.AlterColumn<long>(
                name: "CustomerAssetsID",
                table: "Order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_CustomerAssets_CustomerAssetsID",
                table: "Order",
                column: "CustomerAssetsID",
                principalTable: "CustomerAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
