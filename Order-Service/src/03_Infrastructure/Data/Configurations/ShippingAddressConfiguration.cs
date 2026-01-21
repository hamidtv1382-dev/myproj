//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Order_Service.src._01_Domain.Core.ValueObjects;

//namespace Order_Service.src._03_Infrastructure.Data.Configurations
//{
//    public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
//    {
//        public void Configure(EntityTypeBuilder<ShippingAddress> builder)
//        {
//            builder.ToTable("ShippingAddresses");

//            builder.Property(sa => sa.FirstName).IsRequired().HasMaxLength(100);
//            builder.Property(sa => sa.LastName).IsRequired().HasMaxLength(100);
//            builder.Property(sa => sa.PhoneNumber).IsRequired().HasMaxLength(20);
//            builder.Property(sa => sa.Country).IsRequired().HasMaxLength(100);
//            builder.Property(sa => sa.City).IsRequired().HasMaxLength(100);
//            builder.Property(sa => sa.State).IsRequired().HasMaxLength(100);
//            builder.Property(sa => sa.Street).IsRequired().HasMaxLength(255);
//            builder.Property(sa => sa.ZipCode).IsRequired().HasMaxLength(20);
//            builder.Property(sa => sa.BuildingNumber).HasMaxLength(50);
//            builder.Property(sa => sa.ApartmentNumber).HasMaxLength(50);
//            builder.Property(sa => sa.PostalCodeAdditional).HasMaxLength(50);
//        }
//    }
//}
