using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Windetta.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class AddedMaxLengthForAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Wallets",
                type: "CHAR(48)",
                maxLength: 48,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Wallets",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "CHAR(48)",
                oldMaxLength: 48)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
