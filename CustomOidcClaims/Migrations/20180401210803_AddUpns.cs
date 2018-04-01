using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomOidcClaims.Migrations
{
    public partial class AddUpns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPrincipalName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPrincipalName",
                table: "Users");
        }
    }
}
