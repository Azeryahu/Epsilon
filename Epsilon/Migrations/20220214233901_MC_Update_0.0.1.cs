using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class MC_Update_001 : Migration
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
                    MinecraftID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCLocations", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MCLocations");
        }
    }
}
