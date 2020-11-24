using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyIt.DataAccess.Migrations
{
    public partial class add_chat_entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chatroom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatroom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chatroom_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatroomMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatroomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatroomMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatroomMessage_Chatroom_ChatroomId",
                        column: x => x.ChatroomId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatroomMessage_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChatroom",
                columns: table => new
                {
                    ChatroomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatroom", x => new { x.UserId, x.ChatroomId });
                    table.ForeignKey(
                        name: "FK_UserChatroom_Chatroom_ChatroomId",
                        column: x => x.ChatroomId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChatroom_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chatroom_FlightId",
                table: "Chatroom",
                column: "FlightId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatroomMessage_ChatroomId",
                table: "ChatroomMessage",
                column: "ChatroomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatroomMessage_UserId",
                table: "ChatroomMessage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatroom_ChatroomId",
                table: "UserChatroom",
                column: "ChatroomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatroomMessage");

            migrationBuilder.DropTable(
                name: "UserChatroom");

            migrationBuilder.DropTable(
                name: "Chatroom");
        }
    }
}