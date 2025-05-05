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
                restaurantId = restaurantId,
                name = "Test Restaurant",
                address = "123 Test St",
                neighborhood = "Downtown",
                cuisine = "Italian",
                priceRange = 'M',
                dietaryOptions = "Vegetarian options",
                createdAt = DateTime.Now
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
            Assert.AreEqual(restaurant.restaurantId, returnedRestaurant!.restaurantId);
            Assert.AreEqual(restaurant.name, returnedRestaurant.name);
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
                new Restaurant { restaurantId = 1, name = "Restaurant 1", cuisine = "Italian", priceRange = 'M' },
                new Restaurant { restaurantId = 2, name = "Restaurant 2", cuisine = "French", priceRange = 'H' }
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