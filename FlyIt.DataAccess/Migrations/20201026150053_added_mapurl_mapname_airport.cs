using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class added_mapurl_mapname_airport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapName",
                table: "Airport",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "Airport",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapName",
                table: "Airport");

            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "Airport");
        }
    }
}
