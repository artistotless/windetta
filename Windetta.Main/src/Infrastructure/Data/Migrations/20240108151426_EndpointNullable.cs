using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EndpointNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: true,
                collation: "latin1_general_ci",
                oldClrType: typeof(string),
                oldType: "VARCHAR(42)",
                oldCollation: "latin1_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MatchFlow",
                keyColumn: "Endpoint",
                keyValue: null,
                column: "Endpoint",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: false,
                collation: "latin1_general_ci",
                oldClrType: typeof(string),
                oldType: "VARCHAR(42)",
                oldNullable: true,
                oldCollation: "latin1_general_ci");
        }
    }
}
