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
            var restaurant = new Restaurant { restaurantId = restaurantId };
            var reviews = new List<Review>
            {
                new Review { reviewId = 1, userId = 1, restaurantId = restaurantId, rating = 4, comment = "Great place!" },
                new Review { reviewId = 2, userId = 2, restaurantId = restaurantId, rating = 5, comment = "Amazing food!" }
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
            var user = new User { userId = userId };
            var restaurant = new Restaurant { restaurantId = restaurantId };
            var review = new Review { reviewId = 1, userId = userId, restaurantId = restaurantId, rating = 4, comment = "Great!" };
            
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
            Assert.AreEqual(review.reviewId, returnedReview!.reviewId);
        }

        [TestMethod]
        public void CreateReview_ValidReview_ReturnsCreatedReview()
        {
            // Arrange
            var review = new Review
            {
                userId = 1,
                restaurantId = 1,
                rating = 4,
                comment = "Great restaurant!"
            };
            
            var user = new User { userId = review.userId };
            var restaurant = new Restaurant { restaurantId = review.restaurantId };
            
            _mockUserRepository.Setup(r => r.GetUserById(review.userId)).Returns(user);
            _mockRestaurantRepository.Setup(r => r.GetById(review.restaurantId)).Returns(restaurant);
            _mockReviewRepository.Setup(r => r.GetUserReviewForRestaurant(review.userId, review.restaurantId)).Returns((Review?)null);
            _mockReviewRepository.Setup(r => r.InsertReview(It.IsAny<Review>()))
                .Callback<Review>(r => r.reviewId = 1)
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
            Assert.AreEqual(1, returnedReview!.reviewId);
        }
    }
}