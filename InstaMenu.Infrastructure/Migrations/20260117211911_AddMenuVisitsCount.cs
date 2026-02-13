using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstaMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuVisitsCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuVisitsCount",
                table: "merchant_settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuVisitsCount",
                table: "merchant_settings");
        }
    }
}
