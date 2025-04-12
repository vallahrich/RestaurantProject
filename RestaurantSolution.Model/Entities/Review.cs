using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSolution.Model.Entities
{
    public class Review
    {
        public Review()
        {
        }

        public Review(int id)
        {
            reviewId = id;
        }

        public int reviewId { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        public int restaurantId { get; set; }

        [Required]
        [Range(1, 5)]
        public short rating { get; set; }

        public string comment { get; set; }

        public DateTime createdAt { get; set; } = DateTime.Now;

        // Navigation properties
        [JsonIgnore]  // This prevents serialization issues
        public User User { get; set; }

        [JsonIgnore]
        public Restaurant Restaurant { get; set; }
    }
}