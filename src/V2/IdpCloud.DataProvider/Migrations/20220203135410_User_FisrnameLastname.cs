using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class User_FisrnameLastname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_PersonId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Persons",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Users_PersonId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PersonId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Firstname",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Lastname",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                schema: "Identity",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Persons",
                schema: "Identity",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryLivingId = table.Column<short>(type: "smallint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Middlename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalityId = table.Column<short>(type: "smallint", nullable: true),
                    PicturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryLivingId",
                        column: x => x.CountryLivingId,
                        principalSchema: "BaseInfo",
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_NationalityId",
                        column: x => x.NationalityId,
                        principalSchema: "BaseInfo",
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Persons_Softwares_SoftwareId",
                        column: x => x.SoftwareId,
                        principalSchema: "Identity",
                        principalTable: "Softwares",
                        principalColumn: "SoftwareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                schema: "Identity",
                table: "Users",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryLivingId",
                schema: "Identity",
                table: "Persons",
                column: "CountryLivingId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NationalityId",
                schema: "Identity",
                table: "Persons",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_SoftwareId",
                schema: "Identity",
                table: "Persons",
                column: "SoftwareId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_PersonId",
                schema: "Identity",
                table: "Users",
                column: "PersonId",
                principalSchema: "Identity",
                principalTable: "Persons",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
