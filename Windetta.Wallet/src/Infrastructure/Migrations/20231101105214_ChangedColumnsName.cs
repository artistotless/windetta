using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class ChangedColumnsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nanotons",
                table: "Transactions",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "Nanotons");
        }
    }
}
