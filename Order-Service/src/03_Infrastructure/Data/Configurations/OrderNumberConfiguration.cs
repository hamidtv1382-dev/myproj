//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Order_Service.src._01_Domain.Core.ValueObjects;

//namespace Order_Service.src._03_Infrastructure.Data.Configurations
//{
//    public class OrderNumberConfiguration : IEntityTypeConfiguration<OrderNumber>
//    {
//        public void Configure(EntityTypeBuilder<OrderNumber> builder)
//        {
//            builder.ToTable("OrderNumbers");
//            builder.Property(on => on.Value).HasMaxLength(50);
//        }
//    }
//}
