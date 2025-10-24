using InstaMenu.Domain.Entities;
using InstaMenu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaMenu.Infrastructure.Presistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.CustomerName)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("varchar(100)");

            builder.Property(o => o.CustomerPhone)
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasColumnType("varchar(20)");

            builder.Property(o => o.CustomerAddress)
                   .HasMaxLength(300)
                   .HasColumnType("varchar(300)");

            builder.Property(o => o.ItemsJson)
                   .IsRequired()
                   .HasColumnType("jsonb"); // PostgreSQL native JSON type

            builder.Property(o => o.TotalPrice)
                   .HasColumnType("numeric(10,2)") // PostgreSQL decimal format
                   .HasPrecision(10, 2);

            builder.Property(o => o.SentToWhatsapp)
                   .HasDefaultValue(false)
                   .HasColumnType("boolean");

            builder.Property(o => o.CreatedAt)
                   .HasColumnType("timestamp with time zone")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(o => o.Notes)
                   .HasMaxLength(2000)
                   .HasColumnType("varchar(2000)");

            // Configure the Status enum
            builder.Property(o => o.Status)
                   .HasConversion<int>()
                   .HasDefaultValue(OrderStatus.Pending);

            builder.HasOne(o => o.Merchant)
                   .WithMany(r => r.Orders)
                   .HasForeignKey(o => o.MerchantId)
                   .OnDelete(DeleteBehavior.Restrict); // PostgreSQL best practice
        }
    }
}
