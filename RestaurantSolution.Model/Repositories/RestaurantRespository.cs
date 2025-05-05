using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class RestaurantRepository : BaseRepository, IRestaurantRepository
    {
        public RestaurantRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Restaurant GetById(int id)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM restaurants WHERE restaurant_id = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

            var data = GetData(dbConn, cmd);
            if (data != null && data.Read())
            {
                return new Restaurant(Convert.ToInt32(data["restaurant_id"]))
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
            }
            return null;
        }

        public List<Restaurant> FilterRestaurants(string[] neighborhoods = null, string[] cuisines = null,
            char[] priceRanges = null, string[] dietaryOptions = null)
        {
            using var dbConn = new NpgsqlConnection(ConnectionString);
            var restaurants = new List<Restaurant>();
            var cmd = dbConn.CreateCommand();

            var whereConditions = new List<string>();

            // Handle neighborhoods filter
            if (neighborhoods != null && neighborhoods.Length > 0)
            {
                var neighborhoodParams = new List<string>();
                for (int i = 0; i < neighborhoods.Length; i++)
                {
                    string paramName = $"@neighborhood{i}";
                    neighborhoodParams.Add($"neighborhood ILIKE {paramName}");
                    cmd.Parameters.Add(paramName, NpgsqlDbType.Text).Value = $"%{neighborhoods[i]}%";
                }
                whereConditions.Add($"({string.Join(" OR ", neighborhoodParams)})");
            }

            // Handle cuisines filter
            if (cuisines != null && cuisines.Length > 0)
            {
                var cuisineParams = new List<string>();
                for (int i = 0; i < cuisines.Length; i++)
                {
                    string paramName = $"@cuisine{i}";
                    cuisineParams.Add($"cuisine ILIKE {paramName}");
                    cmd.Parameters.Add(paramName, NpgsqlDbType.Text).Value = $"%{cuisines[i]}%";
                }
                whereConditions.Add($"({string.Join(" OR ", cuisineParams)})");
            }

            // Handle price ranges filter
            if (priceRanges != null && priceRanges.Length > 0)
            {
                var priceParams = new List<string>();
                for (int i = 0; i < priceRanges.Length; i++)
                {
                    string paramName = $"@priceRange{i}";
                    priceParams.Add($"price_range = {paramName}");
                    cmd.Parameters.Add(paramName, NpgsqlDbType.Char).Value = priceRanges[i].ToString();
                }
                whereConditions.Add($"({string.Join(" OR ", priceParams)})");
            }

            // Handle dietary options filter
            if (dietaryOptions != null && dietaryOptions.Length > 0)
            {
                var dietaryParams = new List<string>();
                for (int i = 0; i < dietaryOptions.Length; i++)
                {
                    string paramName = $"@dietaryOption{i}";
                    dietaryParams.Add($"dietary_options ILIKE {paramName}");
                    cmd.Parameters.Add(paramName, NpgsqlDbType.Text).Value = $"%{dietaryOptions[i]}%";
                }
                whereConditions.Add($"({string.Join(" OR ", dietaryParams)})");
            }

            var whereClause = whereConditions.Count > 0 ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";
            cmd.CommandText = $"SELECT * FROM restaurants {whereClause}";

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