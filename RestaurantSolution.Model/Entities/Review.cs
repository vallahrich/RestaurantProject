/// <summary>
/// Entity class representing a user's review for a restaurant.
/// Stores ratings, comments, and metadata for restaurant feedback.
/// </summary>
/// <remarks>
/// This entity captures user opinions about restaurants, forming the foundation
/// for the rating and review system in the application.
/// 
/// Key features:
/// - Stores a numerical rating (1-5 scale) with validation constraints
/// - Captures textual feedback through optional comments
/// - Links to both user and restaurant entities via navigation properties
/// - Tracks when the review was created
/// - Uses [JsonIgnore] to prevent circular references in API responses
/// 
/// The class enforces data integrity through:
/// - Required fields for essential relationships (UserId, RestaurantId)
/// - Range validation on ratings (1-5 stars)
/// 
/// This entity supports the application's review functionality including:
/// - Displaying restaurant ratings and reviews
/// - Calculating average ratings for restaurants
/// - Showing a user's review history
/// - Allowing users to create, update, and delete their reviews
/// </remarks>

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