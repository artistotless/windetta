using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedReadyToConnectStateProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanceledReason",
                table: "MatchFlow");

            migrationBuilder.AddColumn<int>(
                name: "ReadyToConnectState",
                table: "MatchFlow",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadyToConnectState",
                table: "MatchFlow");

            migrationBuilder.AddColumn<string>(
                name: "CanceledReason",
                table: "MatchFlow",
                type: "VARCHAR(32)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
