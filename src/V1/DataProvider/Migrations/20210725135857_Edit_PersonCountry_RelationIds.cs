using Microsoft.EntityFrameworkCore.Migrations;

namespace DataProvider.Migrations
{
    public partial class Edit_PersonCountry_RelationIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_CountryLivingCountryId",
                schema: "Identity",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_NationalityCountryId",
                schema: "Identity",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_CountryLivingCountryId",
                schema: "Identity",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_NationalityCountryId",
                schema: "Identity",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CountryLivingCountryId",
                schema: "Identity",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "NationalityCountryId",
                schema: "Identity",
                table: "Persons");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CountryLivingCountryId",
                schema: "Identity",
                table: "Persons",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "NationalityCountryId",
                schema: "Identity",
                table: "Persons",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryLivingCountryId",
                schema: "Identity",
                table: "Persons",
                column: "CountryLivingCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NationalityCountryId",
                schema: "Identity",
                table: "Persons",
                column: "NationalityCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_CountryLivingCountryId",
                schema: "Identity",
                table: "Persons",
                column: "CountryLivingCountryId",
                principalSchema: "BaseInfo",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_NationalityCountryId",
                schema: "Identity",
                table: "Persons",
                column: "NationalityCountryId",
                principalSchema: "BaseInfo",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
