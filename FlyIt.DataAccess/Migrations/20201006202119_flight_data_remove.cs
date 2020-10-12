using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace FlyIt.DataAccess.Migrations
{
    public partial class flight_data_remove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flight_DepartureDestination_DepartureId",
                table: "Flight");

            migrationBuilder.DropForeignKey(
                name: "FK_Flight_DepartureDestination_DestinationId",
                table: "Flight");

            migrationBuilder.DropTable(
                name: "DepartureDestination");

            migrationBuilder.DropIndex(
                name: "IX_Flight_DepartureId",
                table: "Flight");

            migrationBuilder.DropIndex(
                name: "IX_Flight_DestinationId",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureId",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Flight");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DepartureActual",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureAirportName",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartureDelay",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DepartureEstimated",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureGate",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureIata",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DepartureScheduled",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureTerminal",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DestinationActual",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationAirportName",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationDelay",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DestinationEstimated",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationGate",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationIata",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DestinationScheduled",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationTerminal",
                table: "Flight",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureActual",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureAirportName",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureDelay",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureEstimated",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureGate",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureIata",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureScheduled",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DepartureTerminal",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationActual",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationAirportName",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationDelay",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationEstimated",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationGate",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationIata",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationScheduled",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "DestinationTerminal",
                table: "Flight");

            migrationBuilder.AddColumn<string>(
                name: "DepartureId",
                table: "Flight",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationId",
                table: "Flight",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DepartureDestination",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Actual = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AirportName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Delay = table.Column<int>(type: "int", nullable: true),
                    Estimated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Gate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scheduled = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Terminal = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureDestination", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flight_DepartureId",
                table: "Flight",
                column: "DepartureId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_DestinationId",
                table: "Flight",
                column: "DestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_DepartureDestination_DepartureId",
                table: "Flight",
                column: "DepartureId",
                principalTable: "DepartureDestination",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_DepartureDestination_DestinationId",
                table: "Flight",
                column: "DestinationId",
                principalTable: "DepartureDestination",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}