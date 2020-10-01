using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace FlyIt.DataAccess.Migrations
{
    public partial class Roles_Add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO dbo.Role(Name,NormalizedName,ConcurrencyStamp) VALUES ('AirportsAdministrator', 'AIRPORTSADMINISTRATOR', '{Guid.NewGuid()}')");

            migrationBuilder.Sql($"INSERT INTO dbo.Role(Name,NormalizedName,ConcurrencyStamp) VALUES ('SystemAdministrator', 'SYSTEMADMINISTRATOR', '{Guid.NewGuid()}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.Role WHERE Name = 'AirportsAdministrator'");

            migrationBuilder.Sql("DELETE FROM dbo.Role WHERE Name = 'SystemAdministrator'");
        }
    }
}
