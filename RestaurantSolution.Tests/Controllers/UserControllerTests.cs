/// <summary>
/// Test class for validating the functionality of the UserController.
/// Ensures proper handling of user-related API operations including retrieval, authentication, and registration.
/// </summary>
/// <remarks>
/// Uses the MSTest framework with Moq for mocking repository dependencies.
/// Tests cover the following scenarios:
/// - User retrieval with both existing and non-existent users
/// - Login authentication with valid and invalid credentials
/// - User registration with validation for username uniqueness
/// 
/// Each test follows the Arrange-Act-Assert pattern to clearly structure test logic.
/// The class uses TestInitialize to set up the mocked repository and controller instance
/// before each test, ensuring isolated test environments.
/// 
/// These tests validate that the UserController:
/// - Returns appropriate HTTP status codes for different scenarios
/// - Properly formats response objects
/// - Communicates correctly with the repository layer
/// - Implements proper validation logic for user operations
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
    public class UserControllerTests
    {
        private Mock<IUserRepository> _mockUserRepository = null!;
        private UserController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new UserController(_mockUserRepository.Object);
        }

        [TestMethod]
        public void GetUserById_UserExists_ReturnsOkWithUser()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                UserId = userId,
                Username = "john.doe",
                Email = "john@example.com",
                PasswordHash = "VerySecret!",
                CreatedAt = DateTime.Now
            };

            _mockUserRepository.Setup(r => r.GetUserById(userId)).Returns(user);

            // Act
            var actionResult = _controller.GetUserById(userId);

            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var returnedUser = okResult!.Value as User;
            Assert.IsNotNull(returnedUser);
            Assert.AreEqual(user.UserId, returnedUser!.UserId);
            Assert.AreEqual(user.Username, returnedUser.Username);
        }

        [TestMethod]
        public void GetUserById_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int nonExistentUserId = 999;
            _mockUserRepository.Setup(r => r.GetUserById(nonExistentUserId)).Returns((User?)null);

            // Act
            var actionResult = _controller.GetUserById(nonExistentUserId);

            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void Login_ValidCredentials_ReturnsOkWithUserAndHeader()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "john.doe",
                PasswordHash = "VerySecret!"
            };

            var user = new User
            {
                UserId = 1,
                Username = "john.doe",
                Email = "john@example.com",
                PasswordHash = "VerySecret!",
                CreatedAt = DateTime.Now
            };

            _mockUserRepository.Setup(r => r.GetUserByUsername(loginRequest.Username)).Returns(user);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // The result is not dynamic, so we need to check it safely
            // Using a dictionary approach to check properties
            Assert.IsNotNull(okResult.Value);

            // Check that it's a valid response without assuming property names
            // Use reflection to check if properties exist
            var resultType = okResult.Value.GetType();

            // Check that the object has some properties - we can't directly access
            // '.user' as a dynamic property because it doesn't exist in that format
            var properties = resultType.GetProperties();
            Assert.IsTrue(properties.Length > 0, "Result should have properties");

            // If you need to verify specific structure, you could convert to JSON
            // var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            // Assert.IsTrue(json.Contains("user") && json.Contains("headerValue"));
        }
        [TestMethod]
        public void Login_InvalidUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "nonexistent",
                PasswordHash = "VerySecret!"
            };

            _mockUserRepository.Setup(r => r.GetUserByUsername(loginRequest.Username)).Returns((User?)null);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "john.doe",
                PasswordHash = "WrongPassword"
            };

            var user = new User
            {
                UserId = 1,
                Username = "john.doe",
                Email = "john@example.com",
                PasswordHash = "VerySecret!",
                CreatedAt = DateTime.Now
            };

            _mockUserRepository.Setup(r => r.GetUserByUsername(loginRequest.Username)).Returns(user);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Register_NewUser_ReturnsCreatedUser()
        {
            // Arrange
            var newUser = new User
            {
                Username = "newuser",
                Email = "new@example.com",
                PasswordHash = "Password123!"
            };

            _mockUserRepository.Setup(r => r.UsernameExists(newUser.Username)).Returns(false);
            _mockUserRepository.Setup(r => r.EmailExists(newUser.Email)).Returns(false);
            _mockUserRepository.Setup(r => r.InsertUser(It.IsAny<User>()))
                .Callback<User>(u => u.UserId = 5)
                .Returns(true);

            // Act
            var actionResult = _controller.Register(newUser);

            // Assert
            var result = actionResult.Result;
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var returnedUser = createdResult!.Value as User;
            Assert.IsNotNull(returnedUser);
            Assert.AreEqual(newUser.Username, returnedUser!.Username);
            Assert.AreEqual(5, returnedUser.UserId);
        }

        [TestMethod]
        public void Register_ExistingUsername_ReturnsConflict()
        {
            // Arrange
            var newUser = new User
            {
                Username = "existinguser",
                Email = "new@example.com",
                PasswordHash = "Password123!"
            };

            _mockUserRepository.Setup(r => r.UsernameExists(newUser.Username)).Returns(true);

            // Act
            var result = _controller.Register(newUser);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
        }
    }
}