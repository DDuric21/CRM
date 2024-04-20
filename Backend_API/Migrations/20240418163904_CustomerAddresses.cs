using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class CustomerAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "IsLegal",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsLegal = table.Column<bool>(type: "bit", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    AddressID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "IsLegal",
                table: "Addresses");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
