using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeletedBetProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CanceledReason",
                table: "MatchFlow",
                type: "VARCHAR(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "BetAmount",
                table: "MatchFlow",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<int>(
                name: "BetCurrencyId",
                table: "MatchFlow",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BetAmount",
                table: "MatchFlow");

            migrationBuilder.DropColumn(
                name: "BetCurrencyId",
                table: "MatchFlow");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "MatchFlow",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(42)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CanceledReason",
                table: "MatchFlow",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(32)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
