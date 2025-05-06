/// <summary>
/// Base abstract repository class that provides foundational database access functionality.
/// Acts as the parent class for all specific repository implementations in the application.
/// </summary>
/// <remarks>
/// This class centralizes common database operations and connection management:
/// - Handles connection string retrieval and validation
/// - Provides standardized methods for database CRUD operations:
///   - GetData: Executes SELECT queries and returns data readers
///   - InsertData: Executes INSERT commands
///   - UpdateData: Executes UPDATE commands
///   - DeleteData: Executes DELETE commands
/// - Ensures consistent connection handling across all repository implementations
/// - Uses Npgsql provider for PostgreSQL database access
/// 
/// All derived repositories inherit these core capabilities, promoting code reuse
/// and consistent database access patterns throughout the application.
/// </remarks>

using Microsoft.Extensions.Configuration;
using Npgsql;

namespace RestaurantSolution.Model.Repositories
{
    public class BaseRepository
    {
        protected string ConnectionString { get; }
        
        public BaseRepository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("RestaurantDB")??
                throw new ArgumentException("Connection string not found");
        }
        
        protected NpgsqlDataReader GetData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            conn.Open();
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        
        protected bool InsertData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            conn.Open();
            cmd.ExecuteNonQuery();
            return true;
        }
        
        protected bool UpdateData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            conn.Open();
            cmd.ExecuteNonQuery();
            return true;
        }
        
        protected bool DeleteData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            conn.Open();
            cmd.ExecuteNonQuery();
            return true;
        }
    }
}