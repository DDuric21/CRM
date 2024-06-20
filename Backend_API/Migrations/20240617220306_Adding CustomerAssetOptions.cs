using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class AddingCustomerAssetOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerAssetOptions",
                columns: table => new
                {
                    CustomerAssetsID = table.Column<long>(type: "bigint", nullable: false),
                    OptionID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAssetOptions", x => new { x.CustomerAssetsID, x.OptionID });
                    table.ForeignKey(
                        name: "FK_CustomerAssetOptions_CustomerAssets_CustomerAssetsID",
                        column: x => x.CustomerAssetsID,
                        principalTable: "CustomerAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAssetOptions_Options_OptionID",
                        column: x => x.OptionID,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssetOptions_CustomerAssetsID",
                table: "CustomerAssetOptions",
                column: "CustomerAssetsID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAssetOptions_OptionID",
                table: "CustomerAssetOptions",
                column: "OptionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerAssetOptions");
        }
    }
}
