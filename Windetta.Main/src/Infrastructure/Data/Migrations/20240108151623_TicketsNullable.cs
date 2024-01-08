using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TicketsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tickets",
                table: "MatchFlow",
                type: "longtext",
                nullable: true,
                collation: "latin1_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldCollation: "latin1_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MatchFlow",
                keyColumn: "Tickets",
                keyValue: null,
                column: "Tickets",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Tickets",
                table: "MatchFlow",
                type: "longtext",
                nullable: false,
                collation: "latin1_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldCollation: "latin1_general_ci");
        }
    }
}
