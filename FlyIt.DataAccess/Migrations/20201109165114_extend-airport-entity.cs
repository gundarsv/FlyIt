using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class extendairportentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmergencyPhoneNo",
                table: "Airport",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxiPhoneNo",
                table: "Airport",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyPhoneNo",
                table: "Airport");

            migrationBuilder.DropColumn(
                name: "TaxiPhoneNo",
                table: "Airport");
        }
    }
}
