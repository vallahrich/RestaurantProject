/// <summary>
/// Controller that handles restaurant review operations through a RESTful API interface.
/// Provides endpoints for creating, retrieving, updating, and deleting restaurant reviews.
/// </summary>
/// <remarks>
/// Exposes the following API endpoints:
/// - GET /api/review/restaurant/{restaurantId}: Retrieve all reviews for a specific restaurant
/// - GET /api/review/user/{userId}/restaurant/{restaurantId}: Get a user's specific review for a restaurant
/// - POST /api/review: Create a new review with validation for one review per user per restaurant
/// - PUT /api/review: Update an existing review
/// - DELETE /api/review/user/{userId}/restaurant/{restaurantId}: Remove a user's review for a restaurant
/// 
/// The controller performs validation including:
/// - Confirming users and restaurants exist before operations
/// - Ensuring users can only have one review per restaurant
/// - Verifying users can only modify their own reviews
/// - Validating review data through model state validation
/// 
/// All endpoints use appropriate HTTP status codes for RESTful compliance.
/// </remarks>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;

        public ReviewController(IReviewRepository reviewRepository, IUserRepository userRepository, IRestaurantRepository restaurantRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
        }

        // GET: api/review/restaurant/{restaurantId}
        [AllowAnonymous]
        [HttpGet("restaurant/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Review>> GetReviewsByRestaurantId(int restaurantId)
        {
            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found");
            }

            var reviews = _reviewRepository.GetReviewsByRestaurantId(restaurantId);
            return Ok(reviews);
        }

        // GET: api/review/user/{userId}/restaurant/{restaurantId}
        [HttpGet("user/{userId}/restaurant/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Review> GetUserReviewForRestaurant(int userId, int restaurantId)
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

            var review = _reviewRepository.GetUserReviewForRestaurant(userId, restaurantId);
            if (review == null)
            {
                return NotFound($"Review for restaurant ID {restaurantId} by user ID {userId} not found");
            }

            return Ok(review);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<Review> CreateReview(Review review)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user exists
            var user = _userRepository.GetUserById(review.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {review.UserId} not found");
            }

            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(review.RestaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {review.RestaurantId} not found");
            }

            // Check if user already has a review for this restaurant
            var existingReview = _reviewRepository.GetUserReviewForRestaurant(review.UserId, review.RestaurantId);
            if (existingReview != null)
            {
                return Conflict($"User already has a review for this restaurant");
            }

            // Clear navigation properties to avoid validation issues
            review.User = null;
            review.Restaurant = null;

            bool success = _reviewRepository.InsertReview(review);
            if (!success)
            {
                return BadRequest("Failed to create review");
            }

            return CreatedAtAction(
                nameof(GetUserReviewForRestaurant),
                new { userId = review.UserId, restaurantId = review.RestaurantId },
                review
            );
        }

        // PUT: api/review
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateReview(Review review)
        {
            // Check if the review exists and belongs to this user
            var existingReview = _reviewRepository.GetUserReviewForRestaurant(review.UserId, review.RestaurantId);
            if (existingReview == null)
            {
                return NotFound("Review not found or doesn't belong to this user");
            }

            // Update with the existing review ID
            review.ReviewId = existingReview.ReviewId;

            bool success = _reviewRepository.UpdateReview(review);
            if (!success)
            {
                return BadRequest("Failed to update review");
            }

            return Ok(review);
        }

        // DELETE: api/review/user/{userId}/restaurant/{restaurantId}
        [HttpDelete("user/{userId}/restaurant/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteUserReviewForRestaurant(int userId, int restaurantId)
        {
            // Check if the review exists
            var existingReview = _reviewRepository.GetUserReviewForRestaurant(userId, restaurantId);
            if (existingReview == null)
            {
                return NotFound("Review not found");
            }

            bool success = _reviewRepository.DeleteReview(existingReview.ReviewId, userId);
            if (!success)
            {
                return BadRequest("Failed to delete review");
            }

            return NoContent();
        }
    }
}