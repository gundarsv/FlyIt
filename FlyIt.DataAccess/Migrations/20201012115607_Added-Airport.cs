using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class AddedAirport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Iata = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAirport",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    AirportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAirport", x => new { x.UserId, x.AirportId });
                    table.ForeignKey(
                        name: "FK_UserAirport_Airport_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAirport_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAirport_AirportId",
                table: "UserAirport",
                column: "AirportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAirport");

            migrationBuilder.DropTable(
                name: "Airport");
        }
    }
}
