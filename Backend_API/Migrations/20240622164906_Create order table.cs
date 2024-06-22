using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class Createordertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerAssetOptions_CustomerAssetsID",
                table: "CustomerAssetOptions");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerAssetsID = table.Column<long?>(type: "bigint", nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Order_CustomerAssets_CustomerAssetsID",
                        column: x => x.CustomerAssetsID,
                        principalTable: "CustomerAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerAssetsID",
                table: "Order",
                column: "CustomerAssetsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssetOptions_CustomerAssetsID",
                table: "CustomerAssetOptions",
                column: "CustomerAssetsID");
        }
    }
}
