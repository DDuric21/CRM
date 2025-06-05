using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_API.Migrations
{
    public partial class CreateBasePermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "DateCreated", "DateModified", "Name" },
                values: new object[,]
                {
                { DateTime.UtcNow, DateTime.UtcNow, "read_user" },
                { DateTime.UtcNow, DateTime.UtcNow, "edit_user" },
                { DateTime.UtcNow, DateTime.UtcNow, "delete_user" },
                { DateTime.UtcNow, DateTime.UtcNow, "create_user" },
                { DateTime.UtcNow, DateTime.UtcNow, "read_customer" },
                { DateTime.UtcNow, DateTime.UtcNow, "edit_customer" },
                { DateTime.UtcNow, DateTime.UtcNow, "delete_customer" },
                { DateTime.UtcNow, DateTime.UtcNow, "create_customer" },
                { DateTime.UtcNow, DateTime.UtcNow, "read_asset" },
                { DateTime.UtcNow, DateTime.UtcNow, "edit_asset" },
                { DateTime.UtcNow, DateTime.UtcNow, "delete_asset" },
                { DateTime.UtcNow, DateTime.UtcNow, "create_asset" },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Name",
                keyValues: new object[] { "read_user", "edit_user", "delete_user", "create_user", "read_customer", "edit_customer", "delete_customer", "create_customer", "read_asset", "edit_asset", "delete_asset", "create_asset" });
        }
    }
}
