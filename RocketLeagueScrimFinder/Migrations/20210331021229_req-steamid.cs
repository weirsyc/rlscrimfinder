using Microsoft.EntityFrameworkCore.Migrations;

namespace RocketLeagueScrimFinder.Migrations
{
    public partial class reqsteamid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SteamId",
                table: "ScrimRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamId",
                table: "ScrimRequests");
        }
    }
}
