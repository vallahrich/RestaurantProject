/// <summary>
/// Test class for validating the functionality of the RestaurantController.
/// Ensures proper handling of restaurant retrieval operations including getting by ID and filtering.
/// </summary>
/// <remarks>
/// Uses the MSTest framework with Moq for mocking repository dependencies.
/// Tests cover the following scenarios:
/// - Retrieving a restaurant by ID when it exists
/// - Handling requests for non-existent restaurants
/// - Filtering restaurants with no specified criteria
/// 
/// Each test follows the Arrange-Act-Assert pattern to clearly structure test logic.
/// The class uses TestInitialize to set up the mocked repositories and controller instance
/// before each test, ensuring isolated test environments.
/// 
/// These tests validate that the RestaurantController:
/// - Returns appropriate HTTP status codes for different scenarios
/// - Properly formats response objects
/// - Communicates correctly with repository dependencies
/// - Processes restaurant data correctly for client consumption
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
    public class BookmarkControllerTests
    {
        private Mock<IBookmarkRepository> _mockBookmarkRepository = null!;
        private Mock<IUserRepository> _mockUserRepository = null!;
        private Mock<IRestaurantRepository> _mockRestaurantRepository = null!;
        private BookmarkController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockBookmarkRepository = new Mock<IBookmarkRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRestaurantRepository = new Mock<IRestaurantRepository>();
            
            _controller = new BookmarkController(
                _mockBookmarkRepository.Object,
                _mockUserRepository.Object,
                _mockRestaurantRepository.Object
            );
        }

        [TestMethod]
        public void GetBookmarkedRestaurants_UserExists_ReturnsRestaurants()
        {
            // Arrange
            int userId = 1;
            var user = new User { UserId = userId };
            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" }
            };
            
            _mockUserRepository.Setup(r => r.GetUserById(userId)).Returns(user);
            _mockBookmarkRepository.Setup(r => r.GetBookmarkedRestaurants(userId)).Returns(restaurants);
            
            // Act
            var actionResult = _controller.GetBookmarkedRestaurants(userId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedRestaurants = okResult!.Value as IEnumerable<Restaurant>;
            Assert.IsNotNull(returnedRestaurants);
            Assert.AreEqual(2, ((List<Restaurant>)returnedRestaurants).Count);
        }

        [TestMethod]
        public void GetBookmarkedRestaurants_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int nonExistentUserId = 999;
            _mockUserRepository.Setup(r => r.GetUserById(nonExistentUserId)).Returns((User?)null);
            
            // Act
            var actionResult = _controller.GetBookmarkedRestaurants(nonExistentUserId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void IsBookmarked_UserAndRestaurantExist_ReturnsBookmarkStatus()
        {
            // Arrange
            int userId = 1;
            int restaurantId = 1;
            var user = new User { UserId = userId };
            var restaurant = new Restaurant { RestaurantId = restaurantId };
            bool isBookmarked = true;
            
            _mockUserRepository.Setup(r => r.GetUserById(userId)).Returns(user);
            _mockRestaurantRepository.Setup(r => r.GetById(restaurantId)).Returns(restaurant);
            _mockBookmarkRepository.Setup(r => r.IsBookmarked(userId, restaurantId)).Returns(isBookmarked);
            
            // Act
            var actionResult = _controller.IsBookmarked(userId, restaurantId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedStatus = (bool)okResult!.Value;
            Assert.AreEqual(isBookmarked, returnedStatus);
        }

        [TestMethod]
        public void AddBookmark_ValidBookmark_ReturnsCreated()
        {
            // Arrange
            var bookmark = new Bookmark
            {
                UserId = 1,
                RestaurantId = 1,
                CreatedAt = DateTime.Now
            };
            
            var user = new User { UserId = bookmark.UserId };
            var restaurant = new Restaurant { RestaurantId = bookmark.RestaurantId };
            
            _mockUserRepository.Setup(r => r.GetUserById(bookmark.UserId)).Returns(user);
            _mockRestaurantRepository.Setup(r => r.GetById(bookmark.RestaurantId)).Returns(restaurant);
            _mockBookmarkRepository.Setup(r => r.IsBookmarked(bookmark.UserId, bookmark.RestaurantId)).Returns(false);
            _mockBookmarkRepository.Setup(r => r.AddBookmark(It.IsAny<Bookmark>())).Returns(true);
            
            // Act
            var result = _controller.AddBookmark(bookmark);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public void RemoveBookmark_ExistingBookmark_ReturnsNoContent()
        {
            // Arrange
            int userId = 1;
            int restaurantId = 1;
            var user = new User { UserId = userId };
            var restaurant = new Restaurant { RestaurantId = restaurantId };
            
            _mockUserRepository.Setup(r => r.GetUserById(userId)).Returns(user);
            _mockRestaurantRepository.Setup(r => r.GetById(restaurantId)).Returns(restaurant);
            _mockBookmarkRepository.Setup(r => r.IsBookmarked(userId, restaurantId)).Returns(true);
            _mockBookmarkRepository.Setup(r => r.RemoveBookmark(userId, restaurantId)).Returns(true);
            
            // Act
            var result = _controller.RemoveBookmark(userId, restaurantId);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}