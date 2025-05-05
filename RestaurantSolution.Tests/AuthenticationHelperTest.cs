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