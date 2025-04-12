// In Bookmark.cs
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
        public int userId { get; set; }
        
        [Required]
        public int restaurantId { get; set; }
        
        public DateTime createdAt { get; set; } = DateTime.Now;
        
        // Navigation properties - make nullable and add JsonIgnore
        [JsonIgnore]
        public User? User { get; set; }
        
        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
    }
}