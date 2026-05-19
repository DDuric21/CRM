using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class Addusertoorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserID",
                table: "Order",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedByUserID",
                table: "Order",
                column: "CreatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_CreatedByUserID",
                table: "Order",
                column: "CreatedByUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_CreatedByUserID",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CreatedByUserID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CreatedByUserID",
                table: "Order");
        }
    }
}
