using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tickets",
                table: "MatchFlow");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tickets",
                table: "MatchFlow",
                type: "longtext",
                nullable: true,
                collation: "latin1_general_ci");
        }
    }
}
