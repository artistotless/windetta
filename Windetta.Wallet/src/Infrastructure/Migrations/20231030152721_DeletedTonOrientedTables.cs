using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class DeletedTonOrientedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Wallets");

            migrationBuilder.AddColumn<long>(
                name: "Balance",
                table: "Wallets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Nanotons = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Wallets");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Wallets",
                type: "CHAR(48)",
                maxLength: 48,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PrivateKey = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicKey = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Credentials_Wallets_UserId",
                        column: x => x.UserId,
                        principalTable: "Wallets",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
