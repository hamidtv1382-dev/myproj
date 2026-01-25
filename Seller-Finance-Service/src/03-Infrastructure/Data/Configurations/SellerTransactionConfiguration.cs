using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller_Finance_Service.src._01_Domain.Core.Entities;

namespace Seller_Finance_Service.src._03_Infrastructure.Data.Configurations
{
    public class SellerTransactionConfiguration : IEntityTypeConfiguration<SellerTransaction>
    {
        public void Configure(EntityTypeBuilder<SellerTransaction> builder)
        {
            builder.ToTable("SellerTransactions");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.SellerAccountId).IsRequired();

            // Money Mapping
            builder.Ignore(t => t.Amount);
            builder.Property<decimal>("Amount_Value").HasColumnName("Amount").HasPrecision(18, 0).IsRequired();

            builder.Property(t => t.Type).IsRequired();
            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.ReferenceId).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.Property(t => t.TransactionDate).IsRequired();

            builder.OwnsOne(t => t.AuditInfo, audit =>
            {
                audit.Property(a => a.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
                audit.Property(a => a.CreatedAt).HasColumnName("CreatedAt");
                audit.Property(a => a.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(100);
                audit.Property(a => a.LastModifiedAt).HasColumnName("LastModifiedAt");
            });

            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt);
            builder.Property(t => t.IsDeleted).HasDefaultValue(false);
        }
    }
}
