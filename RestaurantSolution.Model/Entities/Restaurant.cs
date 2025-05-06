/// <summary>
/// Entity class representing a restaurant in the system.
/// Contains comprehensive details about dining establishments for the restaurant explorer application.
/// </summary>
/// <remarks>
/// This core entity stores all restaurant information including:
/// - Basic identification and contact details (name, address)
/// - Location information (neighborhood)
/// - Operational information (opening hours)
/// - Cuisine categorization
/// - Price classification (using L/M/H system for Low/Medium/High price points)
/// - Special dietary accommodations offered
/// - Record creation timestamp
/// 
/// Data validation is enforced through:
/// - Length restrictions on text fields to ensure database compliance
/// - Regular expression validation on the price range field
/// 
/// The class provides two constructors:
/// - Default constructor for framework compatibility
/// - Parameterized constructor for creating instances with a known ID
/// 
/// This entity is used throughout the application for restaurant listing, filtering,
/// and detail display operations.
/// </remarks>

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSolution.Model.Entities
{
    public class Restaurant
    {
        public Restaurant()
        {
        }

        public Restaurant(int id)
        {
            RestaurantId = id;
        }
        
        [JsonPropertyName("restaurantId")]
        public int RestaurantId { get; set; }
        
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [MaxLength(50)]
        [JsonPropertyName("neighborhood")]
        public string Neighborhood { get; set; }
        
        [JsonPropertyName("openingHours")]
        public string OpeningHours { get; set; }
        
        [MaxLength(50)]
        [JsonPropertyName("cuisine")]
        public string Cuisine { get; set; }
        
        // allows on of this stings: L: Low, M: Medium, H: High
        [RegularExpression("^[LMH]$")]
        [JsonPropertyName("priceRange")]
        public char PriceRange { get; set; }
        
        [JsonPropertyName("dietaryOptions")]
        public string DietaryOptions { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}