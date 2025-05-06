/// <summary>
/// Interface defining the contract for Review entity data access operations.
/// Provides methods for retrieving, creating, updating, and deleting restaurant review records.
/// </summary>
/// <remarks>
/// This interface is part of the repository pattern implementation, providing
/// a clear abstraction layer between the application's business logic and data access code.
/// Key responsibilities include:
/// - Retrieving all reviews for a specific restaurant
/// - Getting a specific user's review for a restaurant (one review per user per restaurant)
/// - Creating new review records
/// - Updating existing review content
/// - Removing reviews with appropriate user authorization
/// 
/// Implementing classes will handle the actual database interactions while controllers
/// depend only on this interface, promoting loose coupling and testability.
/// </remarks>

using RestaurantSolution.Model.Entities;

namespace RestaurantSolution.Model.Repositories
{
    public interface IReviewRepository
    {
        List<Review> GetReviewsByRestaurantId(int restaurantId);
        Review GetUserReviewForRestaurant(int userId, int restaurantId);
        bool InsertReview(Review review);
        bool UpdateReview(Review review);
        bool DeleteReview(int id, int userId);
    }
}