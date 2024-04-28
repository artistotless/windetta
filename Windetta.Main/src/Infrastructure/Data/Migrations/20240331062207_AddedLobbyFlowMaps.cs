using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedLobbyFlowMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "MatchFlow");

            migrationBuilder.DropColumn(
                name: "ReadyToConnectState",
                table: "MatchFlow");

            migrationBuilder.CreateTable(
                name: "LobbyFlow",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Players = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BetCurrencyId = table.Column<int>(type: "int", nullable: false),
                    BetAmount = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Properties = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LobbyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Created = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    CurrentState = table.Column<sbyte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobbyFlow", x => x.CorrelationId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LobbyFlow_CorrelationId",
                table: "LobbyFlow",
                column: "CorrelationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LobbyFlow");

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: true,
                collation: "latin1_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "ReadyToConnectState",
                table: "MatchFlow",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
