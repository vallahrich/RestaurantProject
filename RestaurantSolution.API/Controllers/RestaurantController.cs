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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Model.Entities.Restaurant>> GetRestaurants()
        {
            var restaurants = _restaurantRepository.GetRestaurants();
            return Ok(restaurants);
        }

        // GET: api/restaurant/{id}
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

        // GET: api/restaurant/search
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Model.Entities.Restaurant>> SearchRestaurants(
            [FromQuery] string name = null, 
            [FromQuery] string neighborhood = null, 
            [FromQuery] string cuisine = null, 
            [FromQuery] char? priceRange = null)
        {
            var restaurants = _restaurantRepository.SearchRestaurants(name, neighborhood, cuisine, priceRange);
            return Ok(restaurants);
        }

        // GET: api/restaurant/{id}/rating
        [HttpGet("{id}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetRestaurantRating(int id)
        {
            var restaurant = _restaurantRepository.GetById(id);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {id} not found");
            }
            
            var averageRating = _reviewRepository.GetAverageRatingForRestaurant(id);
            return Ok(averageRating);
        }

        // POST: api/restaurant
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Model.Entities.Restaurant> CreateRestaurant(Model.Entities.Restaurant restaurant)
        {
            bool success = _restaurantRepository.InsertRestaurant(restaurant);
            if (!success)
            {
                return BadRequest("Failed to create restaurant");
            }

            return CreatedAtAction(nameof(GetRestaurantById), new { id = restaurant.restaurantId }, restaurant);
        }

        // PUT: api/restaurant
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateRestaurant(Model.Entities.Restaurant restaurant)
        {
            // Check if restaurant exists
            var existingRestaurant = _restaurantRepository.GetById(restaurant.restaurantId);
            if (existingRestaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurant.restaurantId} not found");
            }

            bool success = _restaurantRepository.UpdateRestaurant(restaurant);
            if (!success)
            {
                return BadRequest("Failed to update restaurant");
            }

            return Ok(restaurant);
        }

        // DELETE: api/restaurant/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteRestaurant(int id)
        {
            // Check if restaurant exists
            var existingRestaurant = _restaurantRepository.GetById(id);
            if (existingRestaurant == null)
            {
                return NotFound($"Restaurant with ID {id} not found");
            }

            bool success = _restaurantRepository.DeleteRestaurant(id);
            if (!success)
            {
                return BadRequest("Failed to delete restaurant");
            }

            return NoContent();
        }
    }
}