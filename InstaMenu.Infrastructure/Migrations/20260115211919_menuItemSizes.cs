using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstaMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class menuItemSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Calories",
                table: "MenuItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasMultipleSizes",
                table: "MenuItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PreparationTimeInMinutes",
                table: "MenuItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuItemSizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Calories = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemSizes_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemSizes_MenuItemId",
                table: "MenuItemSizes",
                column: "MenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemSizes");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "HasMultipleSizes",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "PreparationTimeInMinutes",
                table: "MenuItems");
        }
    }
}
