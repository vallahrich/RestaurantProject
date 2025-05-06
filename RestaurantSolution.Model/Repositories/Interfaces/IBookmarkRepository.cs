/// <summary>
/// Interface defining the contract for Bookmark entity data access operations.
/// Provides methods for managing user bookmarks for restaurants.
/// </summary>
/// <remarks>
/// This interface is part of the repository pattern implementation, providing
/// a clear abstraction layer between the application's business logic and data access code.
/// Key responsibilities include:
/// - Retrieving a user's bookmarks
/// - Retrieving details of restaurants bookmarked by a user
/// - Checking if a specific restaurant is bookmarked by a user
/// - Adding new bookmarks
/// - Removing existing bookmarks
/// 
/// Implementing classes will handle the actual database interactions while controllers
/// depend only on this interface, promoting loose coupling and testability.
/// </remarks>

using RestaurantSolution.Model.Entities;

namespace RestaurantSolution.Model.Repositories
{
    public interface IBookmarkRepository
    {
        List<Bookmark> GetBookmarksByUserId(int userId);
        List<Restaurant> GetBookmarkedRestaurants(int userId);
        bool IsBookmarked(int userId, int restaurantId);
        bool AddBookmark(Bookmark bookmark);
        bool RemoveBookmark(int userId, int restaurantId);
    }
}