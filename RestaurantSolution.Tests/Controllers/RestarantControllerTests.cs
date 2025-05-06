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
using System;
using System.Collections.Generic;

namespace RestaurantSolution.Tests.Controllers
{
    [TestClass]
    public class RestaurantControllerTests
    {
        private Mock<IRestaurantRepository> _mockRestaurantRepository = null!;
        private Mock<IReviewRepository> _mockReviewRepository = null!;
        private RestaurantController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRestaurantRepository = new Mock<IRestaurantRepository>();
            _mockReviewRepository = new Mock<IReviewRepository>();
            
            _controller = new RestaurantController(
                _mockRestaurantRepository.Object,
                _mockReviewRepository.Object
            );
        }

        [TestMethod]
        public void GetRestaurantById_RestaurantExists_ReturnsOkWithRestaurant()
        {
            // Arrange
            int restaurantId = 1;
            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                Name = "Test Restaurant",
                Address = "123 Test St",
                Neighborhood = "Downtown",
                Cuisine = "Italian",
                PriceRange = 'M',
                DietaryOptions = "Vegetarian options",
                CreatedAt = DateTime.Now
            };
            
            _mockRestaurantRepository.Setup(r => r.GetById(restaurantId)).Returns(restaurant);
            
            // Act
            var actionResult = _controller.GetRestaurantById(restaurantId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedRestaurant = okResult!.Value as Restaurant;
            Assert.IsNotNull(returnedRestaurant);
            Assert.AreEqual(restaurant.RestaurantId, returnedRestaurant!.RestaurantId);
            Assert.AreEqual(restaurant.Name, returnedRestaurant.Name);
        }

        [TestMethod]
        public void GetRestaurantById_RestaurantDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int nonExistentRestaurantId = 999;
            _mockRestaurantRepository.Setup(r => r.GetById(nonExistentRestaurantId)).Returns((Restaurant?)null);
            
            // Act
            var actionResult = _controller.GetRestaurantById(nonExistentRestaurantId);
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void FilterRestaurants_WithNoFilters_ReturnsAllRestaurants()
        {
            // Arrange
            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1", Cuisine = "Italian", PriceRange = 'M' },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2", Cuisine = "French", PriceRange = 'H' }
            };
            
            _mockRestaurantRepository.Setup(r => r.FilterRestaurants(
                It.IsAny<string[]>(), It.IsAny<string[]>(), It.IsAny<char[]>(), It.IsAny<string[]>()
            )).Returns(restaurants);
            
            // Act
            var actionResult = _controller.FilterRestaurants();
            
            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedRestaurants = okResult!.Value as IEnumerable<Restaurant>;
            Assert.IsNotNull(returnedRestaurants);
            Assert.AreEqual(2, ((List<Restaurant>)returnedRestaurants).Count);
        }
    }
}