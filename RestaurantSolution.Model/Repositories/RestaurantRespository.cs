using Microsoft.Extensions.Configuration;
using RestaurantSolution.Model.Entities;
using Npgsql;
using NpgsqlTypes;

namespace RestaurantSolution.Model.Repositories
{
    public class RestaurantRepository : BaseRepository
    {
        public RestaurantRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Restaurant GetById(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM restaurants WHERE restaurant_id = @id";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new Restaurant(Convert.ToInt32(data["restaurant_id"]))
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
                }
                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }
        public List<Restaurant> FilterRestaurants(string[] neighborhoods = null, string[] cuisines = null,
            char[] priceRanges = null, string[] dietaryOptions = null)
        {
            NpgsqlConnection dbConn = null;
            var restaurants = new List<Restaurant>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
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