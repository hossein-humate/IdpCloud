using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class Add_JobTitle_CompanyName_Persons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                schema: "Identity",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                schema: "Identity",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                schema: "Identity",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                schema: "Identity",
                table: "Persons");
        }
    }
}
