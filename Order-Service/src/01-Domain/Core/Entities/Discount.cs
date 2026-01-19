using Order_Service.src._01_Domain.Core.Common;
using Order_Service.src._01_Domain.Core.Enums;
using Order_Service.src._01_Domain.Core.ValueObjects;

namespace Order_Service.src._01_Domain.Core.Entities
{
    public class Discount : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public DiscountType Type { get; private set; }
        public decimal Value { get; private set; }
        public Money MinimumOrderAmount { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public int UsageLimit { get; private set; }
        public int TimesUsed { get; private set; }
        public bool IsActive { get; private set; }

        protected Discount() { }

        public Discount(Guid id, string code, string description, DiscountType type, decimal value, Money minimumOrderAmount, DateTime? startDate, DateTime? endDate, int usageLimit)
        {
            Id = id;
            Code = code;
            Description = description;
            Type = type;
            Value = value;
            MinimumOrderAmount = minimumOrderAmount;
            StartDate = startDate;
            EndDate = endDate;
            UsageLimit = usageLimit;
            IsActive = true;
            TimesUsed = 0;
        }

        public Money CalculateDiscountAmount(Money orderTotal)
        {
            if (!CanApply(orderTotal))
                return Money.Zero();

            if (Type == DiscountType.Percentage)
            {
                return new Money(orderTotal.Value * (Value / 100));
            }
            else
            {
                return new Money(Value);
            }
        }

        public bool CanApply(Money orderTotal)
        {
            if (!IsActive) return false;
            if (StartDate.HasValue && DateTime.UtcNow < StartDate.Value) return false;
            if (EndDate.HasValue && DateTime.UtcNow > EndDate.Value) return false;
            if (orderTotal.Value < MinimumOrderAmount.Value) return false;
            if (UsageLimit > 0 && TimesUsed >= UsageLimit) return false;

            return true;
        }

        public void Use()
        {
            if (!CanApply(Money.Zero()))
                throw new InvalidOperationException("Discount is not valid or expired.");

            TimesUsed++;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
    }
}
