using InstaMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaMenu.Infrastructure.Presistence.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("varchar(100)");

            builder.Property(m => m.Description)
                   .HasMaxLength(500)
                   .HasColumnType("varchar(500)");

            builder.Property(m => m.Price)
                   .HasColumnType("numeric(10,2)") // PostgreSQL decimal format
                   .HasPrecision(10, 2);

            builder.Property(m => m.ImageUrl)
                   .HasMaxLength(250)
                   .HasColumnType("varchar(250)");

            builder.Property(m => m.IsAvailable)
                   .HasDefaultValue(true)
                   .HasColumnType("boolean");

            builder.HasOne(m => m.Category)
                   .WithMany(c => c.MenuItems)
                   .HasForeignKey(m => m.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // PostgreSQL best practice
        }
    }

}
