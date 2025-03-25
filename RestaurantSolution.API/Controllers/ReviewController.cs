using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Review : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;
        private readonly UserRepository _userRepository;
        private readonly RestaurantRepository _restaurantRepository;

        public Review(ReviewRepository reviewRepository, UserRepository userRepository, RestaurantRepository restaurantRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
        }

        // GET: api/review/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Model.Entities.Review> GetReviewById(int id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
            {
                return NotFound($"Review with ID {id} not found");
            }
            
            return Ok(review);
        }

        // GET: api/review/restaurant/{restaurantId}
        [HttpGet("restaurant/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Model.Entities.Review>> GetReviewsByRestaurantId(int restaurantId)
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

        // GET: api/review/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Model.Entities.Review>> GetReviewsByUserId(int userId)
        {
            // Check if user exists
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }
            
            var reviews = _reviewRepository.GetReviewsByUserId(userId);
            return Ok(reviews);
        }

        // POST: api/review
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Model.Entities.Review> CreateReview(Model.Entities.Review review)
        {
            // Check if user exists
            var user = _userRepository.GetUserById(review.userId);
            if (user == null)
            {
                return NotFound($"User with ID {review.userId} not found");
            }
            
            // Check if restaurant exists
            var restaurant = _restaurantRepository.GetById(review.restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {review.restaurantId} not found");
            }

            bool success = _reviewRepository.InsertReview(review);
            if (!success)
            {
                return BadRequest("Failed to create review");
            }

            return CreatedAtAction(nameof(GetReviewById), new { id = review.reviewId }, review);
        }

        // PUT: api/review
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateReview(Model.Entities.Review review)
        {
            // Check if review exists
            var existingReview = _reviewRepository.GetById(review.reviewId);
            if (existingReview == null)
            {
                return NotFound($"Review with ID {review.reviewId} not found");
            }
            
            // Ensure user owns the review
            if (existingReview.userId != review.userId)
            {
                return BadRequest("You can only update your own reviews");
            }

            bool success = _reviewRepository.UpdateReview(review);
            if (!success)
            {
                return BadRequest("Failed to update review");
            }

            return Ok(review);
        }

        // DELETE: api/review/{id}/user/{userId}
        [HttpDelete("{id}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult DeleteReview(int id, int userId)
        {
            // Check if review exists
            var existingReview = _reviewRepository.GetById(id);
            if (existingReview == null)
            {
                return NotFound($"Review with ID {id} not found");
            }
            
            // Ensure user owns the review
            if (existingReview.userId != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You can only delete your own reviews");
            }

            bool success = _reviewRepository.DeleteReview(id, userId);
            if (!success)
            {
                return BadRequest("Failed to delete review");
            }

            return NoContent();
        }
    }
}