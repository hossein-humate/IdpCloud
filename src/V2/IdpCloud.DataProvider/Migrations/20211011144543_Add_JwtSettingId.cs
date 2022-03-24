using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class Add_JwtSettingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JwtSettings",
                schema: "SSO",
                table: "JwtSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Issuer",
                schema: "SSO",
                table: "JwtSettings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "JwtSettingId",
                schema: "SSO",
                table: "JwtSettings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JwtSettings",
                schema: "SSO",
                table: "JwtSettings",
                column: "JwtSettingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JwtSettings",
                schema: "SSO",
                table: "JwtSettings");

            migrationBuilder.DropColumn(
                name: "JwtSettingId",
                schema: "SSO",
                table: "JwtSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Issuer",
                schema: "SSO",
                table: "JwtSettings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JwtSettings",
                schema: "SSO",
                table: "JwtSettings",
                column: "Issuer");
        }
    }
}
