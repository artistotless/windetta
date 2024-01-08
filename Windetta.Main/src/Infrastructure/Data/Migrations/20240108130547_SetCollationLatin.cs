using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetCollationLatin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tickets",
                table: "MatchFlow",
                type: "longtext",
                nullable: false,
                collation: "latin1_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: false,
                collation: "latin1_general_ci",
                oldClrType: typeof(string),
                oldType: "VARCHAR(42)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tickets",
                table: "MatchFlow",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldCollation: "latin1_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(42)",
                oldCollation: "latin1_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
