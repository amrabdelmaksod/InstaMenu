using InstaMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaMenu.Infrastructure.Presistence.Configurations
{
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("merchants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.NameAr)
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(250);

            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("ix_merchants_slug");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);

            // Relationships
            builder.HasOne(x => x.Settings)
                .WithOne(x => x.Merchant)
                .HasForeignKey<MerchantSettings>(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SocialLinks)
                .WithOne(x => x.Merchant)
                .HasForeignKey(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.Status)
                .HasDatabaseName("ix_merchants_status");

            builder.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("ix_merchants_created_at");

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("ix_merchants_is_deleted");
        }
    }

    public class MerchantSettingsConfiguration : IEntityTypeConfiguration<MerchantSettings>
    {
        public void Configure(EntityTypeBuilder<MerchantSettings> builder)
        {
            builder.ToTable("merchant_settings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
             
                .ValueGeneratedNever();

            builder.Property(x => x.MerchantId)
                .IsRequired();

            builder.HasIndex(x => x.MerchantId)
                .IsUnique()
                .HasDatabaseName("ix_merchant_settings_merchant_id");

            // SEO Properties
            builder.Property(x => x.SeoTitle)
                .HasColumnName("seo_title")
                .HasMaxLength(70);

            builder.Property(x => x.SeoTitleAr)
                .HasMaxLength(70);

            builder.Property(x => x.SeoDescription)
                .HasMaxLength(160);

            builder.Property(x => x.SeoDescriptionAr)
                .HasMaxLength(160);

            // Branding
            builder.Property(x => x.LogoUrl)
                .HasMaxLength(500);

            builder.Property(x => x.CoverImageUrl)
                .HasMaxLength(500);

            // Google Tag Manager
            builder.Property(x => x.GoogleTagManagerId)
                .HasMaxLength(50);

            builder.Property(x => x.IsGoogleTagManagerEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            // About Section
            builder.Property(x => x.AboutUs)
                .HasMaxLength(2000);

            builder.Property(x => x.AboutUsAr)
                .HasMaxLength(2000);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);

            // Relationships
            builder.HasMany(x => x.BusinessHours)
                .WithOne(x => x.MerchantSettings)
                .HasForeignKey(x => x.MerchantSettingsId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("ix_merchant_settings_is_deleted");
        }
    }

    public class BusinessHourConfiguration : IEntityTypeConfiguration<BusinessHour>
    {
        public void Configure(EntityTypeBuilder<BusinessHour> builder)
        {
            builder.ToTable("business_hours");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.MerchantSettingsId)
                .IsRequired();

            builder.Property(x => x.DayOfWeek)
                .IsRequired();

            builder.Property(x => x.OpenTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(x => x.CloseTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(x => x.IsClosed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);

            // Indexes
            builder.HasIndex(x => new { x.MerchantSettingsId, x.DayOfWeek })
                .IsUnique()
                .HasDatabaseName("ix_business_hours_merchant_settings_id_day_of_week");

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("ix_business_hours_is_deleted");
        }
    }

    public class MerchantSocialLinkConfiguration : IEntityTypeConfiguration<MerchantSocialLink>
    {
        public void Configure(EntityTypeBuilder<MerchantSocialLink> builder)
        {
            builder.ToTable("merchant_social_links");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.MerchantId)
                .IsRequired();

            builder.Property(x => x.Platform)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);

            // Indexes
            builder.HasIndex(x => new { x.MerchantId, x.Platform })
                .IsUnique()
                .HasDatabaseName("ix_merchant_social_links_merchant_id_platform");

            builder.HasIndex(x => x.DisplayOrder)
                .HasDatabaseName("ix_merchant_social_links_display_order");

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("ix_merchant_social_links_is_deleted");

            builder.HasIndex(x => x.MerchantId)
                .HasDatabaseName("ix_merchant_social_links_merchant_id");
        }
    }
}