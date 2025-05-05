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
        
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [Required]
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [Required]
        [MaxLength(50)]
        [JsonPropertyName("neighborhood")]
        public string Neighborhood { get; set; }
        
        [JsonPropertyName("openingHours")]
        public string OpeningHours { get; set; }
        
        [Required]
        [MaxLength(50)]
        [JsonPropertyName("cuisine")]
        public string Cuisine { get; set; }
        
        // L: Low, M: Medium, H: High
        [RegularExpression("^[LMH]$")]
        [JsonPropertyName("priceRange")]
        public char PriceRange { get; set; }
        
        [JsonPropertyName("dietaryOptions")]
        public string DietaryOptions { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}