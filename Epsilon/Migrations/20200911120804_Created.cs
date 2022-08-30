using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class Created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bounties",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeCreated = table.Column<DateTimeOffset>(nullable: true),
                    DateTimeCompleted = table.Column<DateTimeOffset>(nullable: true),
                    TimeoutDateTime = table.Column<DateTimeOffset>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    TargetName = table.Column<string>(nullable: true),
                    Payout = table.Column<float>(nullable: false),
                    PointValue = table.Column<int>(nullable: false),
                    BonusPayout = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bounties", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Crews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeFormed = table.Column<DateTimeOffset>(nullable: true),
                    DateTimeDisbanded = table.Column<DateTimeOffset>(nullable: true),
                    CrewStatus = table.Column<bool>(nullable: false),
                    CaptainID = table.Column<decimal>(nullable: false),
                    CaptainUsername = table.Column<string>(nullable: true),
                    NumberOfCrew = table.Column<int>(nullable: false),
                    CrewName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crews", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<decimal>(nullable: false),
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTimeOffset>(nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: true),
                    JobLeadID = table.Column<decimal>(nullable: false),
                    JobLeadUsername = table.Column<string>(nullable: true),
                    JobName = table.Column<string>(nullable: true),
                    JobClassification = table.Column<string>(nullable: true),
                    JobType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Coordinates = table.Column<string>(nullable: true),
                    JobStatus = table.Column<bool>(nullable: false),
                    NumberOfGroups = table.Column<int>(nullable: false),
                    TotalNumberOfSlots = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateFormed = table.Column<DateTimeOffset>(nullable: true),
                    MissionLeadID = table.Column<decimal>(nullable: false),
                    MissionLeadUsername = table.Column<string>(nullable: true),
                    MissionActiveStatus = table.Column<bool>(nullable: false),
                    MissionStartedStatus = table.Column<bool>(nullable: false),
                    NumberOfMembers = table.Column<int>(nullable: false),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: true),
                    MissionName = table.Column<string>(nullable: true),
                    MissionType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Coordinates = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MissionXRefs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MissionID = table.Column<int>(nullable: false),
                    UserID = table.Column<decimal>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false),
                    Ready = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionXRefs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ops",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateFormed = table.Column<DateTimeOffset>(nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: true),
                    OpLeadID = table.Column<decimal>(nullable: false),
                    OpLeadUsername = table.Column<string>(nullable: true),
                    OpActiveStatus = table.Column<bool>(nullable: false),
                    OpStartedStatus = table.Column<bool>(nullable: false),
                    NumberOfMembers = table.Column<int>(nullable: false),
                    Duration = table.Column<double>(nullable: false),
                    OpName = table.Column<string>(nullable: true),
                    OpType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Coordinates = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ops", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OpsXRefs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpID = table.Column<int>(nullable: false),
                    UserID = table.Column<decimal>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false),
                    Ready = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsXRefs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    OrganizationName = table.Column<string>(nullable: true),
                    Standing = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    LastMessageRecieved = table.Column<DateTimeOffset>(nullable: true),
                    DiscordId = table.Column<decimal>(nullable: false),
                    DiscordUserID = table.Column<string>(nullable: true),
                    DiscordUsername = table.Column<string>(nullable: true),
                    DualUsername = table.Column<string>(nullable: true),
                    SteamID = table.Column<string>(nullable: true),
                    SteamUsername = table.Column<string>(nullable: true),
                    VerificationKey = table.Column<string>(nullable: true),
                    NumberOfWarnings = table.Column<int>(nullable: false),
                    NumberOfAttempts = table.Column<int>(nullable: false),
                    PersonalStanding = table.Column<float>(nullable: false),
                    FactionJoinDate = table.Column<DateTimeOffset>(nullable: true),
                    PromotionDate = table.Column<DateTimeOffset>(nullable: true),
                    PromotionPointBalance = table.Column<int>(nullable: false),
                    Branch = table.Column<string>(nullable: true),
                    Rank = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    CanJoin = table.Column<bool>(nullable: false),
                    JoinedFaction = table.Column<bool>(nullable: false),
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
                name: "Bounties");

            migrationBuilder.DropTable(
                name: "Crews");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Indy");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "MissionXRefs");

            migrationBuilder.DropTable(
                name: "Ops");

            migrationBuilder.DropTable(
                name: "OpsXRefs");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
