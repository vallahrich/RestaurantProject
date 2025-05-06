/// <summary>
/// Test class for validating the functionality of the ReviewController.
/// Ensures proper handling of restaurant review operations including retrieval and creation.
/// </summary>
/// <remarks>
/// Uses the MSTest framework with Moq for mocking repository dependencies.
/// Tests cover the following scenarios:
/// - Retrieving all reviews for a restaurant (both when restaurant exists and doesn't exist)
/// - Retrieving a specific user's review for a restaurant
/// - Creating new reviews with validation
/// 
/// Each test follows the Arrange-Act-Assert pattern to clearly structure test logic.
/// The class uses TestInitialize to set up the mocked repositories and controller instance
/// before each test, ensuring isolated test environments.
/// 
/// These tests validate that the ReviewController:
/// - Returns appropriate HTTP status codes for different scenarios
/// - Properly formats response objects
/// - Communicates correctly with multiple repository dependencies
/// - Validates entity existence before performing operations
/// </remarks>

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantSolution.API.Controllers;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.Tests.Controllers
{
    [TestClass]
    public class ReviewControllerTests
    {
        private Mock<IReviewRepository> _mockReviewRepository = null!;
        private Mock<IUserRepository> _mockUserRepository = null!;
        private Mock<IRestaurantRepository> _mockRestaurantRepository = null!;
        private ReviewController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockReviewRepository = new Mock<IReviewRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRestaurantRepository = new Mock<IRestaurantRepository>();
            
            _controller = new ReviewController(
                _mockReviewRepository.Object,
                _mockUserRepository.Object,
                _mockRestaurantRepository.Object
            );
        }

        [TestMethod]
        public void GetReviewsByRestaurantId_RestaurantExists_ReturnsReviews()
        {
            // Arrange
            int restaurantId = 1;
            var restaurant = new Restaurant { RestaurantId = restaurantId };
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, UserId = 1, RestaurantId = restaurantId, Rating = 4, Comment = "Great place!" },
                new Review { ReviewId = 2, UserId = 2, RestaurantId = restaurantId, Rating = 5, Comment = "Amazing food!" }
            };
            
            _mockRestaurantRepository.Setup(r => r.GetById(restaurantId)).Returns(restaurant);
            _mockReviewRepository.Setup(r => r.GetReviewsByRestaurantId(restaurantId)).Returns(reviews);
            
            // Act
            var actionResult = _controller.GetReviewsByRestaurantId(restaurantId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedReviews = okResult!.Value as IEnumerable<Review>;
            Assert.IsNotNull(returnedReviews);
            Assert.AreEqual(2, ((List<Review>)returnedReviews).Count);
        }

        [TestMethod]
        public void GetReviewsByRestaurantId_RestaurantDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int nonExistentRestaurantId = 999;
            _mockRestaurantRepository.Setup(r => r.GetById(nonExistentRestaurantId)).Returns((Restaurant?)null);
            
            // Act
            var actionResult = _controller.GetReviewsByRestaurantId(nonExistentRestaurantId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void GetUserReviewForRestaurant_ReviewExists_ReturnsReview()
        {
            // Arrange
            int userId = 1;
            int restaurantId = 1;
            var user = new User { UserId = userId };
            var restaurant = new Restaurant { RestaurantId = restaurantId };
            var review = new Review { ReviewId = 1, UserId = userId, RestaurantId = restaurantId, Rating = 4, Comment = "Great!" };
            
            _mockUserRepository.Setup(r => r.GetUserById(userId)).Returns(user);
            _mockRestaurantRepository.Setup(r => r.GetById(restaurantId)).Returns(restaurant);
            _mockReviewRepository.Setup(r => r.GetUserReviewForRestaurant(userId, restaurantId)).Returns(review);
            
            // Act
            var actionResult = _controller.GetUserReviewForRestaurant(userId, restaurantId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedReview = okResult!.Value as Review;
            Assert.IsNotNull(returnedReview);
            Assert.AreEqual(review.ReviewId, returnedReview!.ReviewId);
        }

        [TestMethod]
        public void CreateReview_ValidReview_ReturnsCreatedReview()
        {
            // Arrange
            var review = new Review
            {
                UserId = 1,
                RestaurantId = 1,
                Rating = 4,
                Comment = "Great restaurant!"
            };
            
            var user = new User { UserId = review.UserId };
            var restaurant = new Restaurant { RestaurantId = review.RestaurantId };
            
            _mockUserRepository.Setup(r => r.GetUserById(review.UserId)).Returns(user);
            _mockRestaurantRepository.Setup(r => r.GetById(review.RestaurantId)).Returns(restaurant);
            _mockReviewRepository.Setup(r => r.GetUserReviewForRestaurant(review.UserId, review.RestaurantId)).Returns((Review?)null);
            _mockReviewRepository.Setup(r => r.InsertReview(It.IsAny<Review>()))
                .Callback<Review>(r => r.ReviewId = 1)
                .Returns(true);
            
            // Act
            var actionResult = _controller.CreateReview(review);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            
            var returnedReview = createdResult!.Value as Review;
            Assert.IsNotNull(returnedReview);
            Assert.AreEqual(1, returnedReview!.ReviewId);
        }
    }
}