using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class UserPreferenceRepository : BaseRepository
    {
        public UserPreferenceRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public UserPreference GetByUserId(int userId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM user_preferences WHERE user_id = @userId";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                
                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new UserPreference(Convert.ToInt32(data["preference_id"]))
                    {
                        userId = Convert.ToInt32(data["user_id"]),
                        cuisine = data["cuisine"].ToString(),
                        priceRange = data["price_range"].ToString()[0],
                        dietaryOptions = data["dietary_options"].ToString(),
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

        public UserPreference GetById(int preferenceId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM user_preferences WHERE preference_id = @preferenceId";
                cmd.Parameters.Add("@preferenceId", NpgsqlDbType.Integer).Value = preferenceId;
                
                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new UserPreference(Convert.ToInt32(data["preference_id"]))
                    {
                        userId = Convert.ToInt32(data["user_id"]),
                        cuisine = data["cuisine"].ToString(),
                        priceRange = data["price_range"].ToString()[0],
                        dietaryOptions = data["dietary_options"].ToString(),
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

        public bool InsertUserPreference(UserPreference preference)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO user_preferences 
                    (user_id, cuisine, price_range, dietary_options, created_at)
                    VALUES 
                    (@userId, @cuisine, @priceRange, @dietaryOptions, @createdAt)
                    RETURNING preference_id";
                
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, preference.userId);
                cmd.Parameters.AddWithValue("@cuisine", NpgsqlDbType.Text, (object)preference.cuisine ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@priceRange", NpgsqlDbType.Char, preference.priceRange.ToString());
                cmd.Parameters.AddWithValue("@dietaryOptions", NpgsqlDbType.Text, (object)preference.dietaryOptions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                
                dbConn.Open();
                // Get the newly created preference ID
                var preferenceId = Convert.ToInt32(cmd.ExecuteScalar());
                preference.preferenceId = preferenceId;
                
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

        public bool UpdateUserPreference(UserPreference preference)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE user_preferences SET
                    cuisine = @cuisine,
                    price_range = @priceRange,
                    dietary_options = @dietaryOptions
                    WHERE preference_id = @preferenceId";
                
                cmd.Parameters.AddWithValue("@cuisine", NpgsqlDbType.Text, (object)preference.cuisine ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@priceRange", NpgsqlDbType.Char, preference.priceRange.ToString());
                cmd.Parameters.AddWithValue("@dietaryOptions", NpgsqlDbType.Text, (object)preference.dietaryOptions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@preferenceId", NpgsqlDbType.Integer, preference.preferenceId);
                
                bool result = UpdateData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool DeleteUserPreference(int preferenceId)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "DELETE FROM user_preferences WHERE preference_id = @preferenceId";
                cmd.Parameters.AddWithValue("@preferenceId", NpgsqlDbType.Integer, preferenceId);
                
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