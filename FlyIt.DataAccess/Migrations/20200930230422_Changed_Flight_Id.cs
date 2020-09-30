using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class Changed_Flight_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFlight_Flight_FlightId",
                table: "UserFlight");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flight",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "Flight");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Flight",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flight",
                table: "Flight",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFlight_Flight_FlightId",
                table: "UserFlight",
                column: "FlightId",
                principalTable: "Flight",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFlight_Flight_FlightId",
                table: "UserFlight");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flight",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Flight");

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "Flight",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flight",
                table: "Flight",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFlight_Flight_FlightId",
                table: "UserFlight",
                column: "FlightId",
                principalTable: "Flight",
                principalColumn: "FlightId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
