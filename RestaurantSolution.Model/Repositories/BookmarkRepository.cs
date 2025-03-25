using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class BookmarkRepository : BaseRepository
    {
        public BookmarkRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Bookmark> GetBookmarksByUserId(int userId)
        {
            NpgsqlConnection dbConn = null;
            var bookmarks = new List<Bookmark>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
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
                            userId = Convert.ToInt32(data["user_id"]),
                            restaurantId = Convert.ToInt32(data["restaurant_id"]),
                            createdAt = Convert.ToDateTime(data["created_at"])
                        };
                        bookmarks.Add(bookmark);
                    }
                }
                return bookmarks;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool IsBookmarked(int userId, int restaurantId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM bookmarks WHERE user_id = @userId AND restaurant_id = @restaurantId";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                cmd.Parameters.Add("@restaurantId", NpgsqlDbType.Integer).Value = restaurantId;
                
                dbConn.Open();
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool AddBookmark(Bookmark bookmark)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO bookmarks 
                    (user_id, restaurant_id, created_at)
                    VALUES 
                    (@userId, @restaurantId, @createdAt)";
                
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, bookmark.userId);
                cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, bookmark.restaurantId);
                cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                
                bool result = InsertData(dbConn, cmd);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool RemoveBookmark(int userId, int restaurantId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "DELETE FROM bookmarks WHERE user_id = @userId AND restaurant_id = @restaurantId";
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);
                cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, restaurantId);
                
                bool result = DeleteData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public List<Restaurant> GetBookmarkedRestaurants(int userId)
        {
            NpgsqlConnection dbConn = null;
            var restaurants = new List<Restaurant>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
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
                            name = data["name"].ToString(),
                            address = data["address"].ToString(),
                            neighborhood = data["neighborhood"].ToString(),
                            openingHours = data["opening_hours"].ToString(),
                            cuisine = data["cuisine"].ToString(),
                            priceRange = data["price_range"].ToString()[0],
                            dietaryOptions = data["dietary_options"].ToString(),
                            createdAt = Convert.ToDateTime(data["created_at"])
                        };
                        restaurants.Add(restaurant);
                    }
                }
                return restaurants;
            }
            finally
            {
                dbConn?.Close();
            }
        }
    }
}