using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class Update_SoftwareDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "LayerCount",
                schema: "Identity",
                table: "SoftwareDetails",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "LifeState",
                schema: "Identity",
                table: "SoftwareDetails",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TierCount",
                schema: "Identity",
                table: "SoftwareDetails",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LayerCount",
                schema: "Identity",
                table: "SoftwareDetails");

            migrationBuilder.DropColumn(
                name: "LifeState",
                schema: "Identity",
                table: "SoftwareDetails");

            migrationBuilder.DropColumn(
                name: "TierCount",
                schema: "Identity",
                table: "SoftwareDetails");
        }
    }
}
