using RestaurantSolution.Model.Entities;
using System.Collections.Generic;

namespace RestaurantSolution.Model.Repositories
{
    public interface IRestaurantRepository
    {
        Restaurant GetById(int id);
        List<Restaurant> FilterRestaurants(string[] neighborhoods = null, string[] cuisines = null, 
            char[] priceRanges = null, string[] dietaryOptions = null);
    }
}