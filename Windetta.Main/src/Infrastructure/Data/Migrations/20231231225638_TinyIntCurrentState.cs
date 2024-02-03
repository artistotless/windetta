using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TinyIntCurrentState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<sbyte>(
                name: "CurrentState",
                table: "MatchFlow",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CurrentState",
                table: "MatchFlow",
                type: "int",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");
        }
    }
}
