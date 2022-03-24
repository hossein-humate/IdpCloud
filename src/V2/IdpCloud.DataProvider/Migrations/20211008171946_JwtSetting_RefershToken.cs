using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class JwtSetting_RefershToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                schema: "SSO",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JwtSettingId",
                schema: "Identity",
                table: "Softwares",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JwtSettings",
                schema: "SSO",
                columns: table => new
                {
                    JwtSettingId = table.Column<int>(type: "int", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Audience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpireMinute = table.Column<int>(type: "int", nullable: false),
                    HasRefresh = table.Column<bool>(type: "bit", nullable: false),
                    RefreshExpireMinute = table.Column<int>(type: "int", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JwtSettings", x => x.JwtSettingId);
                    table.ForeignKey(
                        name: "FK_JwtSettings_Softwares_SoftwareId",
                        column: x => x.SoftwareId,
                        principalSchema: "Identity",
                        principalTable: "Softwares",
                        principalColumn: "SoftwareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JwtSettings_SoftwareId",
                schema: "SSO",
                table: "JwtSettings",
                column: "SoftwareId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JwtSettings",
                schema: "SSO");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                schema: "SSO",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "JwtSettingId",
                schema: "Identity",
                table: "Softwares");
        }
    }
}
