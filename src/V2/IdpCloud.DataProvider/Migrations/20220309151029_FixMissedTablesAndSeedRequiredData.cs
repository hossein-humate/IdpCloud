using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class FixMissedTablesAndSeedRequiredData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResetPasswords",
                schema: "Security",
                columns: table => new
                {
                    ResetPasswordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Expiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswords", x => x.ResetPasswordId);
                    table.ForeignKey(
                        name: "FK_ResetPasswords_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "Security",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResetPasswordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Decription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_Activities_ResetPasswords_ResetPasswordId",
                        column: x => x.ResetPasswordId,
                        principalSchema: "Security",
                        principalTable: "ResetPasswords",
                        principalColumn: "ResetPasswordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Softwares",
                columns: new[] { "SoftwareId", "ApiKey", "Brand", "BusinessDescription", "CreateDate", "DeleteDate", "KeyExpire", "LogoImage", "Name", "OwnerUserId", "Status", "UpdateDate" },
                values: new object[] { new Guid("133a771c-e319-472f-846d-91828e0f5e09"), "GxCrAPf6anJ20C7O6txzlWjISUB99D6d3+RL2FlE834=", "Basemap", "AAA and SSO", new DateTime(2022, 3, 9, 14, 56, 23, 214, DateTimeKind.Utc).AddTicks(7336), null, new DateTime(2099, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Basemap Identity Provider", new Guid("00000000-0000-0000-0000-000000000000"), 0, new DateTime(2022, 3, 9, 14, 56, 23, 214, DateTimeKind.Utc).AddTicks(7339) });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Users",
                columns: new[] { "UserId", "ConfirmedTermAndCondition", "CreateDate", "DeleteDate", "Description", "Email", "EmailConfirmationExpiry", "EmailConfirmationSecret", "EmailConfirmed", "Firstname", "LanguageId", "LastLoginDate", "LastLoginIp", "Lastname", "LoginTimes", "Mobile", "MobileConfirmed", "OrganisationId", "PasswordHash", "PasswordSalt", "Picture", "RegisterDate", "RegisterIp", "Status", "TwoFactorEnable", "UpdateDate", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000000"), false, new DateTime(2022, 3, 9, 14, 56, 23, 226, DateTimeKind.Utc).AddTicks(4899), new DateTime(2022, 3, 9, 14, 56, 23, 226, DateTimeKind.Utc).AddTicks(4903), null, null, new DateTime(2022, 3, 9, 14, 56, 23, 226, DateTimeKind.Utc).AddTicks(7603), null, false, null, null, null, null, null, 0, null, false, null, null, null, null, new DateTime(2022, 3, 9, 14, 56, 23, 226, DateTimeKind.Utc).AddTicks(6105), "1.0.0.1", 0, false, new DateTime(2022, 3, 9, 14, 56, 23, 226, DateTimeKind.Utc).AddTicks(4902), "Nobody" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "RoleId", "CreateDate", "DeleteDate", "Description", "IsDefault", "Name", "SoftwareId", "UpdateDate" },
                values: new object[] { new Guid("1214885f-8527-48ac-878e-a923761a9027"), new DateTime(2022, 3, 9, 14, 56, 23, 209, DateTimeKind.Utc).AddTicks(8857), null, null, true, "Public", new Guid("133a771c-e319-472f-846d-91828e0f5e09"), new DateTime(2022, 3, 9, 14, 56, 23, 209, DateTimeKind.Utc).AddTicks(8861) });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "RoleId", "CreateDate", "DeleteDate", "Description", "IsDefault", "Name", "SoftwareId", "UpdateDate" },
                values: new object[] { new Guid("1115885f-8527-48ac-878e-a923761a9027"), new DateTime(2022, 3, 9, 14, 56, 23, 210, DateTimeKind.Utc).AddTicks(77), null, null, true, "Administrator", new Guid("133a771c-e319-472f-846d-91828e0f5e09"), new DateTime(2022, 3, 9, 14, 56, 23, 210, DateTimeKind.Utc).AddTicks(77) });

            migrationBuilder.InsertData(
                schema: "SSO",
                table: "JwtSettings",
                columns: new[] { "JwtSettingId", "Audience", "CreateDate", "DeleteDate", "ExpireMinute", "HasRefresh", "Issuer", "RefreshExpireMinute", "Secret", "SoftwareId", "UpdateDate" },
                values: new object[] { 1, "basemap.co.uk", new DateTime(2022, 3, 9, 14, 56, 23, 232, DateTimeKind.Utc).AddTicks(7230), null, 400, true, "basemap.co.uk", 400, "iGfCtXUgK3ulAKvsNu8szl9mUeaYTRhj2EEHCfuTilZ4RKPBRh76YddLUFMxyPRHnZ3XOBvCSMLrokcS6kzpe9pdhonuIdMbXK9X", new Guid("133a771c-e319-472f-846d-91828e0f5e09"), new DateTime(2022, 3, 9, 14, 56, 23, 232, DateTimeKind.Utc).AddTicks(7233) });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ResetPasswordId",
                schema: "Security",
                table: "Activities",
                column: "ResetPasswordId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_UserId",
                schema: "Security",
                table: "Activities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswords_UserId",
                schema: "Security",
                table: "ResetPasswords",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "ResetPasswords",
                schema: "Security");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("1115885f-8527-48ac-878e-a923761a9027"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("1214885f-8527-48ac-878e-a923761a9027"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                schema: "SSO",
                table: "JwtSettings",
                keyColumn: "JwtSettingId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Softwares",
                keyColumn: "SoftwareId",
                keyValue: new Guid("133a771c-e319-472f-846d-91828e0f5e09"));
        }
    }
}
