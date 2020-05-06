using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsilon.Migrations
{
    public partial class IndustryUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateQueued",
                table: "Indy");

            migrationBuilder.AddColumn<decimal>(
                name: "UserID",
                table: "Indy",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Indy");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateQueued",
                table: "Indy",
                nullable: true);
        }
    }
}
