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
        [JsonIgnore]
        public User? User { get; set; }
        
        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
    }
}