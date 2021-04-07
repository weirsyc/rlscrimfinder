using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RocketLeagueScrimFinder.Migrations
{
    public partial class initialdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrimEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SteamId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpponentSteamId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatchmakingPreference = table.Column<int>(type: "int", nullable: false),
                    Collegiate = table.Column<bool>(type: "bit", nullable: false),
                    Servers = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SteamId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrimEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ScrimEvents_ScrimEventId",
                        column: x => x.ScrimEventId,
                        principalTable: "ScrimEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScrimRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScrimEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrimRequests_ScrimEvents_ScrimEventId",
                        column: x => x.ScrimEventId,
                        principalTable: "ScrimEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ScrimEventId",
                table: "ChatMessages",
                column: "ScrimEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrimRequests_ScrimEventId",
                table: "ScrimRequests",
                column: "ScrimEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ScrimRequests");

            migrationBuilder.DropTable(
                name: "ScrimEvents");
        }
    }
}
