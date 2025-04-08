        // public bool InsertRestaurant(Restaurant restaurant)
        // {
        //     NpgsqlConnection dbConn = null;
        //     try
        //     {
        //         dbConn = new NpgsqlConnection(ConnectionString);
        //         var cmd = dbConn.CreateCommand();
        //         cmd.CommandText = @"
        //             INSERT INTO restaurants 
        //             (name, address, neighborhood, opening_hours, cuisine, price_range, dietary_options, created_at)
        //             VALUES 
        //             (@name, @address, @neighborhood, @openingHours, @cuisine, @priceRange, @dietaryOptions, @createdAt)
        //             RETURNING restaurant_id";

        //         cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, restaurant.name);
        //         cmd.Parameters.AddWithValue("@address", NpgsqlDbType.Text, restaurant.address);
        //         cmd.Parameters.AddWithValue("@neighborhood", NpgsqlDbType.Text, restaurant.neighborhood);
        //         cmd.Parameters.AddWithValue("@openingHours", NpgsqlDbType.Text, (object)restaurant.openingHours ?? DBNull.Value);
        //         cmd.Parameters.AddWithValue("@cuisine", NpgsqlDbType.Text, restaurant.cuisine);
        //         cmd.Parameters.AddWithValue("@priceRange", NpgsqlDbType.Char, restaurant.priceRange.ToString());
        //         cmd.Parameters.AddWithValue("@dietaryOptions", NpgsqlDbType.Text, (object)restaurant.dietaryOptions ?? DBNull.Value);
        //         cmd.Parameters.AddWithValue("@createdAt", NpgsqlDbType.TimestampTz, DateTime.UtcNow);

        //         dbConn.Open();
        //         // Get the newly created restaurant ID
        //         var restaurantId = Convert.ToInt32(cmd.ExecuteScalar());
        //         restaurant.restaurantId = restaurantId;

        //         return true;
        //     }
        //     catch (Exception)
        //     {
        //         return false;
        //     }
        //     finally
        //     {
        //         dbConn?.Close();
        //     }
        // }

        // public bool UpdateRestaurant(Restaurant restaurant)
        // {
        //     NpgsqlConnection dbConn = null;
        //     try
        //     {
        //         dbConn = new NpgsqlConnection(ConnectionString);
        //         var cmd = dbConn.CreateCommand();
        //         cmd.CommandText = @"
        //             UPDATE restaurants SET
        //             name = @name,
        //             address = @address,
        //             neighborhood = @neighborhood,
        //             opening_hours = @openingHours,
        //             cuisine = @cuisine,
        //             price_range = @priceRange,
        //             dietary_options = @dietaryOptions
        //             WHERE restaurant_id = @restaurantId";

        //         cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, restaurant.name);
        //         cmd.Parameters.AddWithValue("@address", NpgsqlDbType.Text, restaurant.address);
        //         cmd.Parameters.AddWithValue("@neighborhood", NpgsqlDbType.Text, restaurant.neighborhood);
        //         cmd.Parameters.AddWithValue("@openingHours", NpgsqlDbType.Text, (object)restaurant.openingHours ?? DBNull.Value);
        //         cmd.Parameters.AddWithValue("@cuisine", NpgsqlDbType.Text, restaurant.cuisine);
        //         cmd.Parameters.AddWithValue("@priceRange", NpgsqlDbType.Char, restaurant.priceRange.ToString());
        //         cmd.Parameters.AddWithValue("@dietaryOptions", NpgsqlDbType.Text, (object)restaurant.dietaryOptions ?? DBNull.Value);
        //         cmd.Parameters.AddWithValue("@restaurantId", NpgsqlDbType.Integer, restaurant.restaurantId);

        //         bool result = UpdateData(dbConn, cmd);
        //         return result;
        //     }
        //     finally
        //     {
        //         dbConn?.Close();
        //     }
        // }

        // public bool DeleteRestaurant(int id)
        // {
        //     NpgsqlConnection dbConn = null;
        //     try
        //     {
        //         dbConn = new NpgsqlConnection(ConnectionString);
        //         var cmd = dbConn.CreateCommand();
        //         cmd.CommandText = "DELETE FROM restaurants WHERE restaurant_id = @id";
        //         cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        //         bool result = DeleteData(dbConn, cmd);
        //         return result;
        //     }
        //     finally
        //     {
        //         dbConn?.Close();
        //     }
        // }