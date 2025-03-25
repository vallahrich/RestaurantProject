using System.ComponentModel.DataAnnotations;

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
        
        // Navigation properties
        public User User { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}