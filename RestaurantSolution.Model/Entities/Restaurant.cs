using System.ComponentModel.DataAnnotations;

namespace RestaurantSolution.Model.Entities
{
    public class Restaurant
    {
        public Restaurant()
        {
        }

        public Restaurant(int id)
        {
            restaurantId = id;
        }
        
        public int restaurantId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        
        [Required]
        public string address { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string neighborhood { get; set; }
        
        public string openingHours { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string cuisine { get; set; }
        
        // L: Low, M: Medium, H: High
        [RegularExpression("^[LMH]$")]
        public char priceRange { get; set; }
        
        public string dietaryOptions { get; set; }
        
        public DateTime createdAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        //public ICollection<Review> Reviews { get; set; }
        //public ICollection<Bookmark> Bookmarks { get; set; }
    }
}