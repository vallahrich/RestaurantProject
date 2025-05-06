/// <summary>
/// Controller that handles restaurant bookmark operations through a RESTful API interface.
/// Provides endpoints for managing user bookmarks for favorite restaurants.
/// </summary>
/// <remarks>
/// Exposes the following API endpoints:
/// - GET /api/bookmark/user/{userId}/restaurants: Retrieve all restaurants bookmarked by a specific user
/// - GET /api/bookmark/check/{userId}/{restaurantId}: Check if a specific restaurant is bookmarked by a user
/// - POST /api/bookmark: Create a new bookmark with validation to prevent duplicates
/// - DELETE /api/bookmark/{userId}/{restaurantId}: Remove an existing bookmark
/// 
/// The controller performs validation including:
/// - Confirming users and restaurants exist before operations
/// - Preventing duplicate bookmarks
/// - Validating bookmark data through model state validation
/// - Verifying bookmark existence before removal attempts
/// 
/// All endpoints return appropriate HTTP status codes based on operation outcomes,
/// following standard RESTful conventions for resource management.
/// </remarks>

using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;

        public BookmarkController(IBookmarkRepository bookmarkRepository, IUserRepository userRepository, IRestaurantRepository restaurantRepository)
        {
            _bookmarkRepository = bookmarkRepository;
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
        }

        // GET: api/bookmark/user/{userId}/restaurants
        [HttpGet("user/{userId}/restaurants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Restaurant>> GetBookmarkedRestaurants(int userId)
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
        public ActionResult AddBookmark(Bookmark bookmark)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user exists
            var user = _userRepository.GetUserById(bookmark.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {bookmark.UserId} not found");
            }

            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(bookmark.RestaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {bookmark.RestaurantId} not found");
            }

            // Check if already bookmarked
            if (_bookmarkRepository.IsBookmarked(bookmark.UserId, bookmark.RestaurantId))
            {
                return Conflict($"Restaurant with ID {bookmark.RestaurantId} is already bookmarked by user with ID {bookmark.UserId}");
            }

            // Clear navigation properties to avoid validation issues
            bookmark.User = null;
            bookmark.Restaurant = null;

            bool success = _bookmarkRepository.AddBookmark(bookmark);
            if (!success)
            {
                return BadRequest("Failed to add bookmark");
            }

            return CreatedAtAction(nameof(IsBookmarked), new { userId = bookmark.UserId, restaurantId = bookmark.RestaurantId }, bookmark);
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