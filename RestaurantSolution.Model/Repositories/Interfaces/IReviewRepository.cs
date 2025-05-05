using RestaurantSolution.Model.Entities;
using System.Collections.Generic;

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