using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Epsilon.Migrations
{
    public partial class create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alliances",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllianceDisbandedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AllianceFoundedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AllianceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllianceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllianceStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    MemberCount = table.Column<int>(type: "int", nullable: false),
                    OrganizationCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliances", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Knights",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllianceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllianceStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CompletedMissions = table.Column<int>(type: "int", nullable: false),
                    DaysUntilPromotion = table.Column<int>(type: "int", nullable: false),
                    EnlistmentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OfficerGradDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PersonalStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PointBalance = table.Column<int>(type: "int", nullable: false),
                    RankTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerJoinDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Knights", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BonusEndDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MissionCompletedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MissionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MissionReward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MissionStartDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StandingPercentIncrease = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TennetiveCompletionDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllianceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberCount = table.Column<int>(type: "int", nullable: false),
                    OrganizationDisbandedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OrganizationFoundedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OrganizationID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    OrganiztionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllianceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllianceStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CompletedMissions = table.Column<int>(type: "int", nullable: false),
                    Joined = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PersonalStanding = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ServerJoinDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alliances");

            migrationBuilder.DropTable(
                name: "Knights");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
