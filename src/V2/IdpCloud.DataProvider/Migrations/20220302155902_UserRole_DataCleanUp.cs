using Microsoft.EntityFrameworkCore.Migrations;

namespace IdpCloud.DataProvider.Migrations
{
    public partial class UserRole_DataCleanUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [Identity].[UserRoles](UserRoleId, UserId, RoleId, CreateDate, UpdateDate)
								  (
									SELECT NEWID(),
										   U.UserId,
										   '1214885F-8527-48AC-878E-A923761A9027',
										   GETDATE(),
										   GETDATE()
									FROM [Identity].Users AS U
									WHERE NOT EXISTS(SELECT * FROM [Identity].UserRoles 
									WHERE U.UserId = UserId AND RoleId = '1214885F-8527-48AC-878E-A923761A9027' 
									AND DeleteDate IS NULL)
									AND U.DeleteDate IS NULL
									)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
