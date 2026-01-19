using Microsoft.EntityFrameworkCore;
using Order_Service.src._01_Domain.Core.Entities;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Order_Service.src._03_Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ConfigureValueObjects(modelBuilder);
        }

        private void ConfigureValueObjects(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.OwnsOne(o => o.OrderNumber, navigationBuilder =>
                {
                    navigationBuilder.Property(n => n.Value).HasColumnName("OrderNumber").HasMaxLength(50);
                });

                entity.OwnsOne(o => o.ShippingAddress, navigationBuilder =>
                {
                    navigationBuilder.Property(a => a.FirstName).HasMaxLength(100);
                    navigationBuilder.Property(a => a.LastName).HasMaxLength(100);
                    navigationBuilder.Property(a => a.PhoneNumber).HasMaxLength(20);
                    navigationBuilder.Property(a => a.Country).HasMaxLength(100);
                    navigationBuilder.Property(a => a.City).HasMaxLength(100);
                    navigationBuilder.Property(a => a.State).HasMaxLength(100);
                    navigationBuilder.Property(a => a.Street).HasMaxLength(255);
                    navigationBuilder.Property(a => a.ZipCode).HasMaxLength(20);
                    navigationBuilder.Property(a => a.BuildingNumber).HasMaxLength(50);
                    navigationBuilder.Property(a => a.ApartmentNumber).HasMaxLength(50);
                });

                entity.OwnsOne(o => o.TotalAmount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("TotalAmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("TotalAmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });

                entity.OwnsOne(o => o.DiscountAmount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("DiscountAmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("DiscountAmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });

                entity.OwnsOne(o => o.FinalAmount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("FinalAmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("FinalAmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.OwnsOne(o => o.UnitPrice, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("UnitPriceValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });

            modelBuilder.Entity<Basket>(entity =>
            {
                entity.OwnsOne(o => o.TotalAmount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("TotalAmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("TotalAmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });

            modelBuilder.Entity<BasketItem>(entity =>
            {
                entity.OwnsOne(o => o.UnitPrice, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("UnitPriceValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.OwnsOne(o => o.MinimumOrderAmount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("MinimumOrderAmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("MinimumOrderAmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.OwnsOne(o => o.Amount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("AmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("AmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });

            modelBuilder.Entity<Refund>(entity =>
            {
                entity.OwnsOne(o => o.Amount, navigationBuilder =>
                {
                    navigationBuilder.Property(m => m.Value).HasColumnName("AmountValue").HasColumnType("decimal(18,0)");
                    navigationBuilder.Property(m => m.Currency).HasColumnName("AmountCurrency").HasMaxLength(3).HasDefaultValue("IRR");
                });
            });
        }
    }
}
