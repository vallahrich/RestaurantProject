using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly ReviewRepository _reviewRepository;

        public RestaurantController(RestaurantRepository restaurantRepository, ReviewRepository reviewRepository)
        {
            _restaurantRepository = restaurantRepository;
            _reviewRepository = reviewRepository;
        }

        // GET: api/restaurant/{id}
        [AllowAnonymous] // Add this attribute to make the endpoint public
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Model.Entities.Restaurant> GetRestaurantById(int id)
        {
            var restaurant = _restaurantRepository.GetById(id);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {id} not found");
            }

            return Ok(restaurant);
        }
        
        // GET: api/restaurant
        [AllowAnonymous] // Add this attribute to make the endpoint public
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