using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstaMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatemerchantsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "merchant_settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QRCodeUrl",
                table: "merchant_settings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsAppNumber",
                table: "merchant_settings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "merchant_settings");

            migrationBuilder.DropColumn(
                name: "QRCodeUrl",
                table: "merchant_settings");

            migrationBuilder.DropColumn(
                name: "WhatsAppNumber",
                table: "merchant_settings");
        }
    }
}
