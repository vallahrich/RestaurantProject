using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSolution.Model.Entities
{
    public class User
    {
        // Add a parameterless constructor for JSON deserialization
        public User()
        {
        }

        public User(int id)
        {
            userId = id;
        }
        
        public int userId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string username { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string email { get; set; }
        
        [Required]
        public string passwordHash { get; set; }
        
        public DateTime createdAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        //public UserPreference Preference { get; set; }
        //public ICollection<Review> Reviews { get; set; }
        //public ICollection<Bookmark> Bookmarks { get; set; }
    }
}