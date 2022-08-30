using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class Update001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SteamUsername",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "CompletedMissions",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Enlisted",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedMissions",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Enlisted",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "SteamID",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SteamUsername",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
