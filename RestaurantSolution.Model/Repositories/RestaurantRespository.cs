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

        public List<Restaurant> GetRestaurants()
        {
            NpgsqlConnection dbConn = null;
            var restaurants = new List<Restaurant>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM restaurants";
                
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

        public List<Restaurant> SearchRestaurants(string name = null, string neighborhood = null, string cuisine = null, char? priceRange = null)
        {
            NpgsqlConnection dbConn = null;
            var restaurants = new List<Restaurant>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                
                var whereConditions = new List<string>();
                
                if (!string.IsNullOrEmpty(name))
                {
                    whereConditions.Add("name ILIKE @name");
                    cmd.Parameters.Add("@name", NpgsqlDbType.Text).Value = $"%{name}%";
                }
                
                if (!string.IsNullOrEmpty(neighborhood))
                {
                    whereConditions.Add("neighborhood ILIKE @neighborhood");
                    cmd.Parameters.Add("@neighborhood", NpgsqlDbType.Text).Value = $"%{neighborhood}%";
                }
                
                if (!string.IsNullOrEmpty(cuisine))
                {
                    whereConditions.Add("cuisine ILIKE @cuisine");
                    cmd.Parameters.Add("@cuisine", NpgsqlDbType.Text).Value = $"%{cuisine}%";
                }
                
                if (priceRange.HasValue)
                {
                    whereConditions.Add("price_range = @priceRange");
                    cmd.Parameters.Add("@priceRange", NpgsqlDbType.Char).Value = priceRange.Value.ToString();
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

        public bool InsertRestaurant(Restaurant restaurant)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO restaurants 
                    (name, address, neighborhood, opening_hours, cuisine, price_range, dietary_options, created_at)
                    VALUES 
                    (@name, @address, @neighborhood, @openingHours, @cuisine, @priceRange, @dietaryOptions, @createdAt)
                    RETURNING restaurant_id";
                
                cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, restaurant.name);
                cmd.Parameters.AddWithValue("@address", NpgsqlDbType.Text, restaurant.address);
                cmd.Parameters.AddWithValue("@neighborhood", NpgsqlDbType.Text, restaurant.neighborhood);
                cmd.Parameters.AddWithValue("@openingHours", NpgsqlDbType.Text, (object)restaurant.openingHours ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cuisine", NpgsqlDbType.Text, restaurant.cuisine);
                cmd.Parameters.AddWithValue("@priceRange", NpgsqlDbType.Char, restaurant.priceRange.ToString());
                cmd.Parameters.AddWithValue("@dietaryOptions", NpgsqlDbType.Text, (object)restaurant.dietaryOptions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                
                dbConn.Open();
                // Get the newly created restaurant ID
                var restaurantId = Convert.ToInt32(cmd.ExecuteScalar());
                restaurant.restaurantId = restaurantId;
                
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

        public bool UpdateRestaurant(Restaurant restaurant)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE restaurants SET
                    name = @name,
                    address = @address,
                    neighborhood = @neighborhood,
                    opening_hours = @openingHours,
                    cuisine = @cuisine,
                    price_range = @priceRange,
                    dietary_options = @dietaryOptions
                    WHERE restaurant_id = @restaurantId";
                
                cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, restaurant.name);
                cmd.Parameters.AddWithValue("@address", NpgsqlDbType.Text, restaurant.address);
                cmd.Parameters.AddWithValue("@neighborhood", NpgsqlDbType.Text, restaurant.neighborhood);
                cmd.Parameters.AddWithValue("@openingHours", NpgsqlDbType.Text, (object)restaurant.openingHours ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cuisine", NpgsqlDbType.Text, restaurant.cuisine);
                cmd.Parameters.AddWithValue("@priceRange", NpgsqlDbType.Char, restaurant.priceRange.ToString());
                cmd.Parameters.AddWithValue("@dietaryOptions", NpgsqlDbType.Text, (object)restaurant.dietaryOptions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, restaurant.restaurantId);
                
                bool result = UpdateData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        public bool DeleteRestaurant(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "DELETE FROM restaurants WHERE restaurant_id = @id";
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);
                
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