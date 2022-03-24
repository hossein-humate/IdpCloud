using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class Add_SoftwareDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoftwareDetails",
                schema: "Identity",
                columns: table => new
                {
                    SoftwareDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StagingPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DevelopPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionType = table.Column<byte>(type: "tinyint", nullable: false),
                    ProgrammingLanguage = table.Column<byte>(type: "tinyint", nullable: false),
                    SoftwareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareDetails", x => x.SoftwareDetailId);
                    table.ForeignKey(
                        name: "FK_SoftwareDetails_Softwares_SoftwareId",
                        column: x => x.SoftwareId,
                        principalSchema: "Identity",
                        principalTable: "Softwares",
                        principalColumn: "SoftwareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareDetails_SoftwareId",
                schema: "Identity",
                table: "SoftwareDetails",
                column: "SoftwareId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoftwareDetails",
                schema: "Identity");
        }
    }
}
