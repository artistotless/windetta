using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.TonTxns.Migrations
{
    /// <inheritdoc />
    public partial class AddedTransactionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    TransfersCount = table.Column<short>(type: "SMALLINT", nullable: false),
                    TotalAmount = table.Column<long>(type: "BIGINT", nullable: false),
                    Status = table.Column<sbyte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
