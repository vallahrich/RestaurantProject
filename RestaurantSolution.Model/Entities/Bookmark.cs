/// <summary>
/// Entity class representing a user bookmark for a restaurant.
/// Stores the association between users and their favorite/saved restaurants.
/// </summary>
/// <remarks>
/// This entity forms a many-to-many relationship between users and restaurants,
/// allowing users to save restaurants they're interested in for later reference.
/// 
/// Key features:
/// - Maintains the relationship between a user and a bookmarked restaurant
/// - Tracks when the bookmark was created
/// - Includes navigation properties for efficient entity relationships in Entity Framework
/// - Uses [JsonIgnore] to prevent circular references in API responses
/// - Uses [Required] data annotations to enforce data integrity
/// 
/// The class is used in bookmark operations throughout the application including:
/// - Saving user's favorite restaurants
/// - Retrieving a user's bookmarked restaurants
/// - Checking if a restaurant is bookmarked by a specific user
/// </remarks>

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSolution.Model.Entities
{
    public class Bookmark
    {
        public Bookmark()
        {
        }
        
        [Required]
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [Required]
        [JsonPropertyName("restaurantId")]
        public int RestaurantId { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        /*navigate between related entities without writing explicit joins, 
        oad related data in a single query using Include():*/
        [JsonIgnore]
        public User? User { get; set; }
        
        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
    }
}