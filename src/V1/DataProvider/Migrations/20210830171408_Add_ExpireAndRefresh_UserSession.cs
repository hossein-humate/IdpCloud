using Microsoft.EntityFrameworkCore.Migrations;

namespace DataProvider.Migrations
{
    public partial class Add_ExpireAndRefresh_UserSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpireDate",
                schema: "SSO",
                table: "UserSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                schema: "SSO",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireDate",
                schema: "SSO",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                schema: "SSO",
                table: "UserSessions");
        }
    }
}
