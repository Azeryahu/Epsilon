using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupLeadID = table.Column<decimal>(nullable: false),
                    GroupLeadUsername = table.Column<string>(nullable: true),
                    NumberOfTeams = table.Column<int>(nullable: false),
                    GroupStatus = table.Column<bool>(nullable: false),
                    GroupName = table.Column<string>(nullable: true),
                    JobClass = table.Column<string>(nullable: true),
                    JobType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Indy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateQueued = table.Column<DateTimeOffset>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    PartName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indy", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTimeOffset>(nullable: true),
                    StartDate = table.Column<DateTimeOffset>(nullable: true),
                    JobClass = table.Column<string>(nullable: true),
                    JobType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Coordinates = table.Column<string>(nullable: true),
                    JobHeadID = table.Column<decimal>(nullable: false),
                    JobHeadUsername = table.Column<string>(nullable: true),
                    JobStatus = table.Column<bool>(nullable: false),
                    NumberOfGroups = table.Column<int>(nullable: false),
                    TotalNumberOfSlots = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TeamLeadID = table.Column<decimal>(nullable: false),
                    TeamLeadUsername = table.Column<string>(nullable: true),
                    NumbeOfSlots = table.Column<int>(nullable: false),
                    TeamStatus = table.Column<bool>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    JobClass = table.Column<string>(nullable: true),
                    JobType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    NumberOfWarnings = table.Column<int>(nullable: false),
                    NumberOfAttempts = table.Column<int>(nullable: false),
                    PersonalStanding = table.Column<float>(nullable: false),
                    DiscordId = table.Column<decimal>(nullable: false),
                    DualUsername = table.Column<string>(nullable: true),
                    SteamID = table.Column<string>(nullable: true),
                    SteamUsername = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    VerificationKey = table.Column<string>(nullable: true),
                    FactionJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    PromotionDate = table.Column<DateTimeOffset>(nullable: true),
                    PointBalance = table.Column<int>(nullable: false),
                    DaysUntilPromotion = table.Column<int>(nullable: false),
                    CompletedMissions = table.Column<int>(nullable: false),
                    Branch = table.Column<string>(nullable: true),
                    Rank = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    NDAVerified = table.Column<bool>(nullable: false),
                    CanJoin = table.Column<bool>(nullable: false),
                    JoinedFaction = table.Column<bool>(nullable: false),
                    CurrentOpName = table.Column<string>(nullable: true),
                    CurrentOpType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Indy");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
