using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order_Service.src._01_Domain.Core.Entities;
using Order_Service.src._01_Domain.Core.Enums;

namespace Order_Service.src._03_Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).ValueGeneratedNever();
            builder.Property(o => o.BuyerId).IsRequired();

            builder.Property(o => o.Status)
                .HasConversion(p => p.ToString(),
                              p => (OrderStatus)Enum.Parse(typeof(OrderStatus), p));

            builder.Property(o => o.Description).HasMaxLength(500);
            builder.Property(o => o.OrderDate).IsRequired(false);

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
