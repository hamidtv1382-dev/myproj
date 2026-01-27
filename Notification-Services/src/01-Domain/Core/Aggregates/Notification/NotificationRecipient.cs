using Notification_Services.src._01_Domain.Core.Enums;
using Notification_Services.src._01_Domain.Core.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notification_Services.src._01_Domain.Core.Aggregates.Notification
{
    [Table("NotificationRecipients")]
    public class NotificationRecipient
    {
        public Guid Id { get; private set; }
        public RecipientType Type { get; private set; }

        // Flattened property for EF Core
        public string Contact { get; private set; }

        public bool IsRead { get; private set; }

        // Parameterless constructor for EF Core
        private NotificationRecipient() { }

        // Domain constructor
        public NotificationRecipient(RecipientType type, Core.ValueObjects.RecipientContact contact)
        {
            Id = Guid.NewGuid();
            Type = type;
            Contact = contact.Address; // Extract value from VO
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
