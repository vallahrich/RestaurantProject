using System.ComponentModel.DataAnnotations;

namespace RestaurantSolution.Model.Entities
{
    public class UserPreference
    {
        public UserPreference()
        {
        }

        public UserPreference(int id)
        {
            preferenceId = id;
        }

        public int preferenceId { get; set; }
        
        [Required]
        public int userId { get; set; }
        
        public string cuisine { get; set; }
        
        // L: Low, M: Medium, H: High
        [RegularExpression("^[LMH]$")]
        public char priceRange { get; set; }
        
        public string dietaryOptions { get; set; }
        
        public DateTime createdAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public User User { get; set; }
    }
}