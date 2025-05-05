using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public User GetUserById(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM users WHERE user_id = @id";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                
                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new User(Convert.ToInt32(data["user_id"]))
                    {
                        username = data["username"].ToString(),
                        email = data["email"].ToString(),
                        passwordHash = data["password_hash"].ToString(),
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

        public User GetUserByUsername(string username)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM users WHERE username = @username";
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;
                
                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new User(Convert.ToInt32(data["user_id"]))
                    {
                        username = data["username"].ToString(),
                        email = data["email"].ToString(),
                        passwordHash = data["password_hash"].ToString(),
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

        public bool InsertUser(User user)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO users 
                    (username, email, password_hash, created_at)
                    VALUES 
                    (@username, @email, @passwordHash, @createdAt)
                    RETURNING user_id";
                
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, user.username);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Text, user.email);
                cmd.Parameters.AddWithValue("@passwordHash", NpgsqlDbType.Text, user.passwordHash);
                cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                
                dbConn.Open();
                // Get the newly created user ID
                var userId = Convert.ToInt32(cmd.ExecuteScalar());
                user.userId = userId;
                
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

        public bool UpdateUser(User user)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE users SET
                    username = @username
                    WHERE user_id = @userId";
                
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, user.username);
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, user.userId);
                
                bool result = UpdateData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool UpdateUserPassword(int userId, string passwordHash)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE users SET
                    password_hash = @passwordHash
                    WHERE user_id = @userId";
                
                cmd.Parameters.AddWithValue("@passwordHash", NpgsqlDbType.Text, passwordHash);
                cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);
                
                bool result = UpdateData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool DeleteUser(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "DELETE FROM users WHERE user_id = @id";
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
                
                bool result = DeleteData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool UsernameExists(string username)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, username);
                
                dbConn.Open();
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool EmailExists(string email)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM users WHERE email = @email";
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Text, email);
                
                dbConn.Open();
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            finally
            {
                dbConn?.Close();
            }
        }
    }
}