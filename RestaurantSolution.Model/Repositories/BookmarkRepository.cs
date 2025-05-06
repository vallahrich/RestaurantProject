/// <summary>
/// Repository class that provides data access operations for Bookmark entities.
/// Handles database interactions for saving, retrieving, and managing user bookmarks for restaurants.
/// </summary>
/// <remarks>
/// Implements the IBookmarkRepository interface and inherits from BaseRepository.
/// Uses Npgsql to connect to a PostgreSQL database and perform SQL operations.
/// Key features:
/// - Retrieving a user's bookmarks
/// - Checking if a restaurant is bookmarked by a user
/// - Adding new bookmarks
/// - Removing existing bookmarks
/// - Retrieving full restaurant details for bookmarked restaurants
/// Supports personalized user experience by enabling favorite restaurant tracking.
/// All database connections are automatically managed and disposed through 'using' statements.
/// </remarks>

using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class BookmarkRepository : BaseRepository, IBookmarkRepository
    {
        public BookmarkRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Bookmark> GetBookmarksByUserId(int userId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var bookmarks = new List<Bookmark>();
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM bookmarks WHERE user_id = @userId ORDER BY created_at DESC";
            cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    Bookmark bookmark = new Bookmark
                    {
                        UserId = Convert.ToInt32(data["user_id"]),
                        RestaurantId = Convert.ToInt32(data["restaurant_id"]),
                        CreatedAt = Convert.ToDateTime(data["created_at"])
                    };
                    bookmarks.Add(bookmark);
                }
            }
            return bookmarks;
        }

        public bool IsBookmarked(int userId, int restaurantId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM bookmarks WHERE user_id = @userId AND restaurant_id = @restaurantId";
            cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
            cmd.Parameters.Add("@restaurantId", NpgsqlDbType.Integer).Value = restaurantId;
            
            dbConn.Open();
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public bool AddBookmark(Bookmark bookmark)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO bookmarks 
                (user_id, restaurant_id, created_at)
                VALUES 
                (@userId, @restaurantId, @createdAt)";
            
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, bookmark.UserId);
            cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, bookmark.RestaurantId);
            cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);
            
            try 
            {
                return InsertData(dbConn, cmd);
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveBookmark(int userId, int restaurantId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM bookmarks WHERE user_id = @userId AND restaurant_id = @restaurantId";
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);
            cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, restaurantId);
            
            return DeleteData(dbConn, cmd);
        }

        public List<Restaurant> GetBookmarkedRestaurants(int userId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var restaurants = new List<Restaurant>();
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                SELECT r.* 
                FROM restaurants r
                JOIN bookmarks b ON r.restaurant_id = b.restaurant_id
                WHERE b.user_id = @userId
                ORDER BY b.created_at DESC";
            cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
            
            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    Restaurant restaurant = new Restaurant(Convert.ToInt32(data["restaurant_id"]))
                    {
                        Name = data["name"].ToString(),
                        Address = data["address"].ToString(),
                        Neighborhood = data["neighborhood"].ToString(),
                        OpeningHours = data["opening_hours"].ToString(),
                        Cuisine = data["cuisine"].ToString(),
                        PriceRange = data["price_range"].ToString()[0],
                        DietaryOptions = data["dietary_options"].ToString(),
                        CreatedAt = Convert.ToDateTime(data["created_at"])
                    };
                    restaurants.Add(restaurant);
                }
            }
            return restaurants;
        }
    }
}