using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Poputi.DataAccess.Migrations
{
    public partial class RouteType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RouteMatchId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityRouteType",
                table: "CityRoutes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "CityRoutes",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "RouteMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteMatchType = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: true),
                    MatchedCityRouteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteMatches_CityRoutes_MatchedCityRouteId",
                        column: x => x.MatchedCityRouteId,
                        principalTable: "CityRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RouteMatches_Users_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RouteMatchId",
                table: "Users",
                column: "RouteMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteMatches_DriverId",
                table: "RouteMatches",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteMatches_MatchedCityRouteId",
                table: "RouteMatches",
                column: "MatchedCityRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RouteMatches_RouteMatchId",
                table: "Users",
                column: "RouteMatchId",
                principalTable: "RouteMatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RouteMatches_RouteMatchId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "RouteMatches");

            migrationBuilder.DropIndex(
                name: "IX_Users_RouteMatchId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RouteMatchId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CityRouteType",
                table: "CityRoutes");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "CityRoutes");
        }
    }
}
