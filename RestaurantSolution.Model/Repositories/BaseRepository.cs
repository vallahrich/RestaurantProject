using Microsoft.Extensions.Configuration;
using Npgsql;

namespace RestaurantSolution.Model.Repositories
{
    public class BaseRepository
    {
        protected string ConnectionString { get; }
        
        public BaseRepository(IConfiguration configuration)
        {
            ConnectionString = configuration["ConnectionStrings:RestaurantDB"] ?? 
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