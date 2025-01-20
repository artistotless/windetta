using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserBalances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "HeldBalance",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Balances",
                columns: table => new
                {
                    WalletId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<long>(type: "BIGINT", nullable: false),
                    HeldAmount = table.Column<long>(type: "BIGINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balances", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_Balances_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Balances");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Transactions");

            migrationBuilder.AddColumn<long>(
                name: "Balance",
                table: "Wallets",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "HeldBalance",
                table: "Wallets",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
