/// <summary>
/// Repository class that provides data access operations for Review entities.
/// Handles database interactions for restaurant reviews including retrieving, creating, updating, and deleting review records.
/// </summary>
/// <remarks>
/// Implements the IReviewRepository interface and inherits from BaseRepository.
/// Uses Npgsql to connect to a PostgreSQL database and perform SQL operations.
/// Provides methods for review management:
/// - Retrieving all reviews for a specific restaurant
/// - Retrieving a specific user's review for a restaurant
/// - Creating new reviews
/// - Updating existing reviews
/// - Deleting reviews
/// All database connections are automatically managed and disposed through 'using' statements.
/// Reviews are secured by requiring user authentication for modifications (enforced with user_id checks).
/// </remarks>

using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class ReviewRepository : BaseRepository, IReviewRepository
    {
        public ReviewRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Review> GetReviewsByRestaurantId(int restaurantId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var reviews = new List<Review>();
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM reviews WHERE restaurant_id = @restaurantId ORDER BY created_at DESC";
            cmd.Parameters.Add("@restaurantId", NpgsqlDbType.Integer).Value = restaurantId;

            var data = GetData(dbConn, cmd);
            if (data != null)
            {
                while (data.Read())
                {
                    Review review = new Review(Convert.ToInt32(data["review_id"]))
                    {
                        UserId = Convert.ToInt32(data["user_id"]),
                        RestaurantId = Convert.ToInt32(data["restaurant_id"]),
                        Rating = Convert.ToInt16(data["rating"]),
                        Comment = data["comment"].ToString(),
                        CreatedAt = Convert.ToDateTime(data["created_at"])
                    };
                    reviews.Add(review);
                }
            }
            return reviews;
        }

        public Review GetUserReviewForRestaurant(int userId, int restaurantId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM reviews WHERE user_id = @userId AND restaurant_id = @restaurantId";
            cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
            cmd.Parameters.Add("@restaurantId", NpgsqlDbType.Integer).Value = restaurantId;

            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                return new Review(Convert.ToInt32(data["review_id"]))
                {
                    UserId = Convert.ToInt32(data["user_id"]),
                    RestaurantId = Convert.ToInt32(data["restaurant_id"]),
                    Rating = Convert.ToInt16(data["rating"]),
                    Comment = data["comment"].ToString(),
                    CreatedAt = Convert.ToDateTime(data["created_at"])
                };
            }
            return null;
        }

        public bool InsertReview(Review review)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO reviews 
                (user_id, restaurant_id, rating, comment, created_at)
                VALUES 
                (@userId, @restaurantId, @rating, @comment, @createdAt)
                RETURNING review_id";

            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, review.UserId);
            cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, review.RestaurantId);
            cmd.Parameters.AddWithValue("@rating", NpgsqlDbType.Smallint, review.Rating);
            cmd.Parameters.AddWithValue("@comment", NpgsqlDbType.Text, (object)review.Comment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);

            dbConn.Open();
            try
            {
                // Get the newly created review ID
                var reviewId = Convert.ToInt32(cmd.ExecuteScalar());
                review.ReviewId = reviewId;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateReview(Review review)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                UPDATE reviews SET
                rating = @rating,
                comment = @comment
                WHERE review_id = @reviewId AND user_id = @userId";

            cmd.Parameters.AddWithValue("@rating", NpgsqlDbType.Smallint, review.Rating);
            cmd.Parameters.AddWithValue("@comment", NpgsqlDbType.Text, (object)review.Comment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@reviewId", NpgsqlDbType.Integer, review.ReviewId);
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, review.UserId);

            return UpdateData(dbConn, cmd);
        }

        public bool DeleteReview(int id, int userId)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM reviews WHERE review_id = @id AND user_id = @userId";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);

            return DeleteData(dbConn, cmd);
        }
    }
}