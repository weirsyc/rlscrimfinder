using Microsoft.EntityFrameworkCore.Migrations;

namespace RocketLeagueScrimFinder.Migrations
{
    public partial class lobbyinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LobbyInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrimEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobbyInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LobbyInfos_ScrimEvents_ScrimEventId",
                        column: x => x.ScrimEventId,
                        principalTable: "ScrimEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LobbyInfos_ScrimEventId",
                table: "LobbyInfos",
                column: "ScrimEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LobbyInfos");
        }
    }
}
