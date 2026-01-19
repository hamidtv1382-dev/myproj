using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order_Service.src._01_Domain.Core.Entities;

namespace Order_Service.src._03_Infrastructure.Data.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable("Baskets");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();

            builder.Property(b => b.BuyerId).IsRequired();
            builder.Property(b => b.ExpiresAt).IsRequired(false);

            builder.HasMany(b => b.Items)
                .WithOne()
                .HasForeignKey("BasketId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(b => b.DomainEvents);
        }
    }
}
