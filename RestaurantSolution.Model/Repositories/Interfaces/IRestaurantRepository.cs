/// <summary>
/// Interface defining the contract for Restaurant entity data access operations.
/// Provides methods for retrieving individual restaurants and filtering restaurant collections.
/// </summary>
/// <remarks>
/// This interface is part of the repository pattern implementation, providing
/// a clear abstraction layer between the application's business logic and data access code.
/// Key responsibilities include:
/// - Retrieving individual restaurants by ID
/// - Searching for restaurants using multiple filtering criteria:
///   - Neighborhood location
///   - Cuisine type
///   - Price range (L/M/H classification)
///   - Dietary options/restrictions
/// 
/// The filtering system allows users to discover restaurants matching their specific preferences.
/// Implementing classes will handle the actual database interactions while controllers
/// depend only on this interface, promoting loose coupling and testability.
/// </remarks>

using RestaurantSolution.Model.Entities;

namespace RestaurantSolution.Model.Repositories
{
    public interface IRestaurantRepository
    {
        Restaurant GetById(int id);
        List<Restaurant> FilterRestaurants(string[] neighborhoods = null, string[] cuisines = null, 
            char[] priceRanges = null, string[] dietaryOptions = null);
    }
}