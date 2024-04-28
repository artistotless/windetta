using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedEndpointField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameServerEndpoint",
                table: "MatchFlow",
                type: "VARCHAR(42)",
                nullable: false,
                defaultValue: "",
                collation: "latin1_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameServerEndpoint",
                table: "MatchFlow");
        }
    }
}
