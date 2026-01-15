using InstaMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaMenu.Infrastructure.Presistence.Configurations
{
    public class MenuItemSizeConfiguration : IEntityTypeConfiguration<MenuItemSize>
    {
        public void Configure(EntityTypeBuilder<MenuItemSize> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Price)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(s => s.Calories)
                .HasMaxLength(50);

            builder.HasOne(s => s.MenuItem)
                .WithMany(m => m.Sizes)
                .HasForeignKey(s => s.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
