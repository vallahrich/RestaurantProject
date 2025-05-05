using System;
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