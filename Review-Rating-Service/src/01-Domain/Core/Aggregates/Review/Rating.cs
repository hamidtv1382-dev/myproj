using Review_Rating_Service.src._01_Domain.Core.Enums;
using Review_Rating_Service.src._01_Domain.Core.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Review_Rating_Service.src._01_Domain.Core.Aggregates.Review
{
    [Table("Ratings")]
    public class Rating
    {
        public Guid Id { get; private set; }
        public RatingType Type { get; private set; }

        // Flattened property for EF Core mapping
        public int Value { get; private set; }

        // Constructor for Domain Logic
        public Rating(RatingType type, Core.ValueObjects.RatingValue value)
        {
            Id = Guid.NewGuid();
            Type = type;
            Value = value.Value;
        }

        // Parameterless constructor for EF Core
        private Rating() { }
    }
}
