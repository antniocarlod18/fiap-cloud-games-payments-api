using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGamesPayments.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldsOrderPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "OrderPayment",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "OrderPayment",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "OrderPayment");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "OrderPayment");
        }
    }
}
