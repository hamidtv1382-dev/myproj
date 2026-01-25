using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller_Finance_Service.src._01_Domain.Core.Entities;

namespace Seller_Finance_Service.src._03_Infrastructure.Data.Configurations
{
    public class SellerAccountConfiguration : IEntityTypeConfiguration<SellerAccount>
    {
        public void Configure(EntityTypeBuilder<SellerAccount> builder)
        {
            builder.ToTable("SellerAccounts");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.SellerId).IsRequired();
            builder.Property(s => s.IsActive).IsRequired();

            // SellerBalance Mapping (Flattened)
            builder.Ignore(s => s.Balance);

            builder.Property<decimal>("AvailableBalanceAmount").HasPrecision(18, 0).IsRequired();
            builder.Property<decimal>("PendingBalanceAmount").HasPrecision(18, 0).IsRequired();
            builder.Property<decimal>("HoldBalanceAmount").HasPrecision(18, 0).IsRequired();

            // BankAccount Mapping
            builder.OwnsOne(s => s.BankAccount, bank =>
            {
                bank.Property(b => b.AccountNumber).HasColumnName("BankAccountNumber").HasMaxLength(50).IsRequired();
                bank.Property(b => b.BankName).HasColumnName("BankName").HasMaxLength(100).IsRequired();
                bank.Property(b => b.ShebaNumber).HasColumnName("ShebaNumber").HasMaxLength(26).IsRequired();
                bank.Property(b => b.AccountHolderName).HasColumnName("AccountHolderName").HasMaxLength(200).IsRequired();
            });

            // AuditInfo Mapping
            builder.OwnsOne(s => s.AuditInfo, audit =>
            {
                audit.Property(a => a.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
                audit.Property(a => a.CreatedAt).HasColumnName("CreatedAt");
                audit.Property(a => a.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(100);
                audit.Property(a => a.LastModifiedAt).HasColumnName("LastModifiedAt");
            });

            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt);
            builder.Property(s => s.IsDeleted).HasDefaultValue(false);
        }
    }
}
