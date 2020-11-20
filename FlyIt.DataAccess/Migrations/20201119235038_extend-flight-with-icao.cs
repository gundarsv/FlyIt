using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class extendflightwithicao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartureIcao",
                table: "Flight",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationIcao",
                table: "Flight",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureIcao",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationIcao",
                table: "Flight");
        }
    }
}