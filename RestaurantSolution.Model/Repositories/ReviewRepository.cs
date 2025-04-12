using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class ReviewRepository : BaseRepository
    {
        public ReviewRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Review> GetReviewsByRestaurantId(int restaurantId)
        {
            NpgsqlConnection dbConn = null;
            var reviews = new List<Review>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
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
                            userId = Convert.ToInt32(data["user_id"]),
                            restaurantId = Convert.ToInt32(data["restaurant_id"]),
                            rating = Convert.ToInt16(data["rating"]),
                            comment = data["comment"].ToString(),
                            createdAt = Convert.ToDateTime(data["created_at"])
                        };
                        reviews.Add(review);
                    }
                }
                return reviews;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public Review GetUserReviewForRestaurant(int userId, int restaurantId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM reviews WHERE user_id = @userId AND restaurant_id = @restaurantId";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                cmd.Parameters.Add("@restaurantId", NpgsqlDbType.Integer).Value = restaurantId;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new Review(Convert.ToInt32(data["review_id"]))
                    {
                        userId = Convert.ToInt32(data["user_id"]),
                        restaurantId = Convert.ToInt32(data["restaurant_id"]),
                        rating = Convert.ToInt16(data["rating"]),
                        comment = data["comment"].ToString(),
                        createdAt = Convert.ToDateTime(data["created_at"])
                    };
                }
                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool InsertReview(Review review)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
            INSERT INTO reviews 
            (user_id, restaurant_id, rating, comment, created_at)
            VALUES 
            (@userId, @restaurantId, @rating, @comment, @createdAt)
            RETURNING review_id";

                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, review.userId);
                cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, review.restaurantId);
                cmd.Parameters.AddWithValue("@rating", NpgsqlDbType.Smallint, review.rating);
                cmd.Parameters.AddWithValue("@comment", NpgsqlDbType.Text, (object)review.comment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);

                dbConn.Open();
                // Get the newly created review ID
                var reviewId = Convert.ToInt32(cmd.ExecuteScalar());
                review.reviewId = reviewId;

                return true;
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

        public bool UpdateReview(Review review)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
            UPDATE reviews SET
            rating = @rating,
            comment = @comment
            WHERE review_id = @reviewId AND user_id = @userId";

                cmd.Parameters.AddWithValue("@rating", NpgsqlDbType.Smallint, review.rating);
                cmd.Parameters.AddWithValue("@comment", NpgsqlDbType.Text, (object)review.comment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@reviewId", NpgsqlDbType.Integer, review.reviewId);
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, review.userId);

                bool result = UpdateData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool DeleteReview(int id, int userId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "DELETE FROM reviews WHERE review_id = @id AND user_id = @userId";
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);

                bool result = DeleteData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }
    }
}
