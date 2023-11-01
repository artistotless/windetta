using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTransactionTableTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "HeldBalance",
                table: "Wallets",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Balance",
                table: "Wallets",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Wallets",
                type: "CHAR(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Type",
                table: "Transactions",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "Transactions",
                type: "DATETIME(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<long>(
                name: "Nanotons",
                table: "Transactions",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Transactions",
                type: "VARCHAR(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "HeldBalance",
                table: "Wallets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");

            migrationBuilder.AlterColumn<long>(
                name: "Balance",
                table: "Wallets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Wallets",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "CHAR(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Transactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "Transactions",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)");

            migrationBuilder.AlterColumn<long>(
                name: "Nanotons",
                table: "Transactions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Transactions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(40)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
