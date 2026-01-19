using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order_Service.src._01_Domain.Core.ValueObjects;

namespace Order_Service.src._03_Infrastructure.Data.Configurations
{
    public class MoneyConfiguration : IEntityTypeConfiguration<Money>
    {
        public void Configure(EntityTypeBuilder<Money> builder)
        {
            builder.ToTable("Money");
            builder.Property(m => m.Value).HasColumnType("decimal(18,0)");
            builder.Property(m => m.Currency).HasMaxLength(3).HasDefaultValue("IRR");
        }
    }
}
