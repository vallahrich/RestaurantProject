/// <summary>
/// Test class for validating the AuthenticationHelper utility methods.
/// Verifies the correct behavior of Basic Authentication encoding and decoding.
/// </summary>
/// <remarks>
/// Contains unit tests for:
/// - Encrypt: Verifies correct transformation of username/password into a properly formatted 
///   Basic Authentication header with Base64 encoding
/// - Decrypt: Verifies correct extraction of username/password credentials from a 
///   Basic Authentication header
/// 
/// Tests follow the Arrange-Act-Assert pattern to clearly structure test logic.
/// These tests ensure the authentication helper methods work as expected for the
/// HTTP Basic Authentication scheme implementation in the middleware.
/// </remarks>

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestaurantSolution.Tests
{
    [TestClass]
    public class AuthenticationHelperTest
    {
        [TestMethod]
        public void EncryptTest()
        {
            // Arrange
            string username = "john.doe";
            string password = "VerySecret!";
            
            // Act
            var header = RestaurantSolution.API.Middleware.AuthenticationHelper.Encrypt(username, password);
            
            // Assert
            Assert.AreEqual("Basic am9obi5kb2U6VmVyeVNlY3JldCE=", header);
        }
        
        [TestMethod]
        public void DecryptTest()
        {
            // Arrange
            string header = "Basic am9obi5kb2U6VmVyeVNlY3JldCE=";
            
            // Act
            RestaurantSolution.API.Middleware.AuthenticationHelper.Decrypt(header, out string username, out string password);
            
            // Assert
            Assert.AreEqual("john.doe", username);
            Assert.AreEqual("VerySecret!", password);
        }
    }
}