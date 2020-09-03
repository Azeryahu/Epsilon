using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class Update003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NDAVerified",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "DiscordUsername");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "DiscordUserID");

            migrationBuilder.RenameColumn(
                name: "PointBalance",
                table: "Users",
                newName: "PromotionPointBalance");

            migrationBuilder.AddColumn<int>(
                name: "CrewID",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CrewName",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentGroupName",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentTeamName",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastMessageRecieved",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrewID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CrewName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrentGroupName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrentTeamName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastMessageRecieved",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PromotionPointBalance",
                table: "Users",
                newName: "PointBalance");

            migrationBuilder.RenameColumn(
                name: "DiscordUsername",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "DiscordUserID",
                table: "Users",
                newName: "UserID");

            migrationBuilder.AddColumn<bool>(
                name: "NDAVerified",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }
    }
}
