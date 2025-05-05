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
            ReviewId = id;
        }

        [JsonPropertyName("reviewId")]
        public int ReviewId { get; set; }

        [Required]
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [Required]
        [JsonPropertyName("restaurantId")]
        public int RestaurantId { get; set; }

        [Required]
        [Range(1, 5)]
        [JsonPropertyName("rating")]
        public short Rating { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
    }
}