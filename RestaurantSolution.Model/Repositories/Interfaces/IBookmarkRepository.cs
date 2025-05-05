using RestaurantSolution.Model.Entities;
using System.Collections.Generic;

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