using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarkController : ControllerBase
    {
        private readonly BookmarkRepository _bookmarkRepository;
        private readonly UserRepository _userRepository;
        private readonly RestaurantRepository _restaurantRepository;

        public BookmarkController(BookmarkRepository bookmarkRepository, UserRepository userRepository, RestaurantRepository restaurantRepository)
        {
            _bookmarkRepository = bookmarkRepository;
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
        }

        // GET: api/bookmark/user/{userId}/restaurants
        [HttpGet("user/{userId}/restaurants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Model.Entities.Restaurant>> GetBookmarkedRestaurants(int userId)
        {
            // Check if user exists
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            var restaurants = _bookmarkRepository.GetBookmarkedRestaurants(userId);
            return Ok(restaurants);
        }

        // GET: api/bookmark/check/{userId}/{restaurantId}
        [HttpGet("check/{userId}/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> IsBookmarked(int userId, int restaurantId)
        {
            // Check if user exists
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found");
            }

            bool isBookmarked = _bookmarkRepository.IsBookmarked(userId, restaurantId);
            return Ok(isBookmarked);
        }

        // POST: api/bookmark
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult AddBookmark(Model.Entities.Bookmark bookmark)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user exists
            var user = _userRepository.GetUserById(bookmark.userId);
            if (user == null)
            {
                return NotFound($"User with ID {bookmark.userId} not found");
            }

            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(bookmark.restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {bookmark.restaurantId} not found");
            }

            // Check if already bookmarked
            if (_bookmarkRepository.IsBookmarked(bookmark.userId, bookmark.restaurantId))
            {
                return Conflict($"Restaurant with ID {bookmark.restaurantId} is already bookmarked by user with ID {bookmark.userId}");
            }

            // Clear navigation properties to avoid validation issues
            bookmark.User = null;
            bookmark.Restaurant = null;

            bool success = _bookmarkRepository.AddBookmark(bookmark);
            if (!success)
            {
                return BadRequest("Failed to add bookmark");
            }

            return CreatedAtAction(nameof(IsBookmarked), new { userId = bookmark.userId, restaurantId = bookmark.restaurantId }, bookmark);
        }

        // DELETE: api/bookmark/{userId}/{restaurantId}
        [HttpDelete("{userId}/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult RemoveBookmark(int userId, int restaurantId)
        {
            // Check if user exists
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found");
            }

            // Check if bookmark exists
            if (!_bookmarkRepository.IsBookmarked(userId, restaurantId))
            {
                return NotFound($"Bookmark not found for user ID {userId} and restaurant ID {restaurantId}");
            }

            bool success = _bookmarkRepository.RemoveBookmark(userId, restaurantId);
            if (!success)
            {
                return BadRequest("Failed to remove bookmark");
            }

            return NoContent();
        }
    }
}