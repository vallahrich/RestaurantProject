using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Restaurant : ControllerBase
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly ReviewRepository _reviewRepository;

        public Restaurant(RestaurantRepository restaurantRepository, ReviewRepository reviewRepository)
        {
            _restaurantRepository = restaurantRepository;
            _reviewRepository = reviewRepository;
        }

        // GET: api/restaurant
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Model.Entities.Restaurant>> FilterRestaurants(
            [FromQuery] string[] neighborhood = null,
            [FromQuery] string[] cuisine = null, 
            [FromQuery] char[] priceRange = null,
            [FromQuery] string[] dietaryOptions = null)
        {
            var restaurants = _restaurantRepository.FilterRestaurants(neighborhood, cuisine, priceRange, dietaryOptions);
            return Ok(restaurants);
        }
    }
}