using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstaMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationForInstaMenuPostgresqlDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "merchants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "merchant_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    seo_title = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    SeoTitleAr = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    SeoDescription = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    SeoDescriptionAr = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GoogleTagManagerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsGoogleTagManagerEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AboutUs = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AboutUsAr = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_merchant_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_merchant_settings_merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "merchant_social_links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_merchant_social_links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_merchant_social_links_merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    CustomerAddress = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    ItemsJson = table.Column<string>(type: "jsonb", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SentToWhatsapp = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalTable: "merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "business_hours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantSettingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_hours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_business_hours_merchant_settings_MerchantSettingsId",
                        column: x => x.MerchantSettingsId,
                        principalTable: "merchant_settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_business_hours_is_deleted",
                table: "business_hours",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_business_hours_merchant_settings_id_day_of_week",
                table: "business_hours",
                columns: new[] { "MerchantSettingsId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_MerchantId",
                table: "Categories",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "ix_merchant_settings_is_deleted",
                table: "merchant_settings",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_merchant_settings_merchant_id",
                table: "merchant_settings",
                column: "MerchantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_merchant_social_links_display_order",
                table: "merchant_social_links",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "ix_merchant_social_links_is_deleted",
                table: "merchant_social_links",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_merchant_social_links_merchant_id",
                table: "merchant_social_links",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "ix_merchant_social_links_merchant_id_platform",
                table: "merchant_social_links",
                columns: new[] { "MerchantId", "Platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_merchants_created_at",
                table: "merchants",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_merchants_is_deleted",
                table: "merchants",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_merchants_slug",
                table: "merchants",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_merchants_status",
                table: "merchants",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MerchantId",
                table: "Orders",
                column: "MerchantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_hours");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "merchant_social_links");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "merchant_settings");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "merchants");
        }
    }
}
