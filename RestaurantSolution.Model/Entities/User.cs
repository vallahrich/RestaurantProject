/// <summary>
/// Entity class representing a user account in the system.
/// Contains core user identity and authentication information.
/// </summary>
/// <remarks>
/// This entity stores essential user data including:
/// - Basic identification (user ID, username)
/// - Contact information (email)
/// - Authentication credentials (password hash)
/// - Account creation timestamp
/// 
/// Data validation is enforced through:
/// - Required fields for essential information
/// - Length restrictions on text fields to ensure database compliance
/// 
/// The class provides two constructors:
/// - Default constructor for framework compatibility
/// - Parameterized constructor for creating instances with a known ID
/// 
/// This entity is central to the application's authentication system and
/// forms relationships with other entities including Bookmarks and Reviews.
/// It supports user account management, authentication, and personalization features.
/// </remarks>

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestaurantSolution.Model.Entities
{
    public class User
    {
        public User()
        {
        }

        public User(int id)
        {
            UserId = id;
        }
        
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}