using InstaMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaMenu.Infrastructure.Presistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(256)
                   .HasColumnType("varchar(256)");

            builder.Property(c => c.SortOrder)
                   .HasDefaultValue(0)
                   .HasColumnType("integer");

            builder.HasOne(c => c.Merchant)
                   .WithMany(r => r.Categories)
                   .HasForeignKey(c => c.MerchantId)
                   .OnDelete(DeleteBehavior.Restrict); // PostgreSQL best practice

            builder.HasMany(c => c.MenuItems)
                   .WithOne(m => m.Category)
                   .HasForeignKey(m => m.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // PostgreSQL best practice
        }
    }

}
