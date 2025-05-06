/// <summary>
/// Repository class that provides data access operations for User entities.
/// Handles database interactions including retrieving, creating, updating, and deleting user records.
/// </summary>
/// <remarks>
/// Implements the IUserRepository interface and inherits from BaseRepository.
/// Uses Npgsql to connect to a PostgreSQL database and perform SQL operations.
/// Provides methods for user authentication, verification, and profile management:
/// - Retrieving users by ID or username
/// - Creating new user accounts
/// - Updating user information
/// - Changing passwords
/// - Deleting user accounts
/// - Checking for username/email uniqueness
/// All database connections are automatically managed and disposed through 'using' statements.
/// </remarks>

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
            using var dbConn = new NpgsqlConnection(ConnectionString);  //using var statement automatically handles connection disposal
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM users WHERE user_id = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
            
            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                return new User(Convert.ToInt32(data["user_id"]))
                {
                    Username = data["username"].ToString(),
                    Email = data["email"].ToString(),
                    PasswordHash = data["password_hash"].ToString(),
                    CreatedAt = Convert.ToDateTime(data["created_at"])
                };
            }
            return null;
        }

        public User GetUserByUsername(string username)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM users WHERE username = @username";
            cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;
            
            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                return new User(Convert.ToInt32(data["user_id"]))
                {
                    Username = data["username"].ToString(),
                    Email = data["email"].ToString(),
                    PasswordHash = data["password_hash"].ToString(),
                    CreatedAt = Convert.ToDateTime(data["created_at"])
                };
            }
            return null;
        }

        public bool InsertUser(User user)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO users 
                (username, email, password_hash, created_at)
                VALUES 
                (@username, @email, @passwordHash, @createdAt)
                RETURNING user_id";
            
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, user.Username);
            cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Text, user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", NpgsqlDbType.Text, user.PasswordHash);
            cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);
            
            dbConn.Open();
            try
            {
                // Get the newly created user ID
                var userId = Convert.ToInt32(cmd.ExecuteScalar());
                user.UserId = userId;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                UPDATE users SET
                username = @username
                WHERE user_id = @userId";
            
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, user.Username);
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, user.UserId);
            
            return UpdateData(dbConn, cmd);
        }

        public bool UpdateUserPassword(int userId, string passwordHash)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                UPDATE users SET
                password_hash = @passwordHash
                WHERE user_id = @userId";
            
            cmd.Parameters.AddWithValue("@passwordHash", NpgsqlDbType.Text, passwordHash);
            cmd.Parameters.AddWithValue("@userId", NpgsqlDbType.Integer, userId);
            
            return UpdateData(dbConn, cmd);
        }

        public bool DeleteUser(int id)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM users WHERE user_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
            
            return DeleteData(dbConn, cmd);
        }

        public bool UsernameExists(string username)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Text, username);
            
            dbConn.Open();
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public bool EmailExists(string email)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM users WHERE email = @email";
            cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Text, email);
            
            dbConn.Open();
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }
}