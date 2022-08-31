using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class Created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MCLocations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTimeOffset>(nullable: true),
                    DateLooted = table.Column<DateTimeOffset>(nullable: true),
                    Discoverer = table.Column<decimal>(nullable: false),
                    Looter = table.Column<decimal>(nullable: false),
                    XCoordinate = table.Column<int>(nullable: false),
                    YCoordinate = table.Column<int>(nullable: false),
                    ZCoordinate = table.Column<int>(nullable: false),
                    ThingFound = table.Column<string>(nullable: true),
                    MinecraftID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCLocations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    LastMessageRecieved = table.Column<DateTimeOffset>(nullable: true),
                    DiscordId = table.Column<decimal>(nullable: false),
                    DiscordUserID = table.Column<string>(nullable: true),
                    DiscordUsername = table.Column<string>(nullable: true),
                    DualUsername = table.Column<string>(nullable: true),
                    VerificationKey = table.Column<string>(nullable: true),
                    NumberOfWarnings = table.Column<int>(nullable: false),
                    NumberOfAttempts = table.Column<int>(nullable: false),
                    PersonalStanding = table.Column<float>(nullable: false),
                    FactionJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    PromotionDate = table.Column<DateTimeOffset>(nullable: true),
                    PromotionPointBalance = table.Column<int>(nullable: false),
                    CompletedMissions = table.Column<int>(nullable: false),
                    Branch = table.Column<string>(nullable: true),
                    Rank = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    CanJoin = table.Column<bool>(nullable: false),
                    JoinedFaction = table.Column<bool>(nullable: false),
                    Enlisted = table.Column<bool>(nullable: false),
                    Verified = table.Column<bool>(nullable: false),
                    CrewID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MCLocations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
