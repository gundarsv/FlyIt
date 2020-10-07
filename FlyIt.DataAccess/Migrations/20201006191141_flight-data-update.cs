using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class flightdataupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartureId",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationId",
                table: "Flight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Flight",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DepartureDestination",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Gate = table.Column<string>(nullable: true),
                    Delay = table.Column<int>(nullable: true),
                    Terminal = table.Column<string>(nullable: true),
                    AirportName = table.Column<string>(nullable: true),
                    Scheduled = table.Column<DateTimeOffset>(nullable: true),
                    Estimated = table.Column<DateTimeOffset>(nullable: true),
                    Actual = table.Column<DateTimeOffset>(nullable: true),
                    Iata = table.Column<string>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Flight");
        }
    }
}
