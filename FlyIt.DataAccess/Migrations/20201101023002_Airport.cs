using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class Airport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RentingCompanyName",
                table: "Airport",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentingCompanyPhoneNo",
                table: "Airport",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RentingCompanyUrl",
                table: "Airport",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentingCompanyName",
                table: "Airport");

            migrationBuilder.DropColumn(
                name: "RentingCompanyPhoneNo",
                table: "Airport");

            migrationBuilder.DropColumn(
                name: "RentingCompanyUrl",
                table: "Airport");
        }
    }
}
