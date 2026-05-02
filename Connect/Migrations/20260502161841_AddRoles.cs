using Connect.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,] {
                    { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Roles.Admin, Roles.Admin.ToUpper() },
                    { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Roles.User, Roles.User.ToUpper() }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles");
        }
    }
}
