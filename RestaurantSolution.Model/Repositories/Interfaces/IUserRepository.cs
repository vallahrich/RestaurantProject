/// <summary>
/// Interface defining the contract for User entity data access operations.
/// Provides methods for retrieving, creating, updating, and deleting user records.
/// </summary>
/// <remarks>
/// This interface is part of the repository pattern implementation, providing
/// a clear abstraction layer between the application's business logic and data access code.
/// Key responsibilities include:
/// - User account management (retrieval, creation, updates, deletion)
/// - User authentication (username/password validation)
/// - Data integrity (username/email uniqueness verification)
/// 
/// Implementing classes will handle the actual database interactions while controllers
/// depend only on this interface, promoting loose coupling, Dependency Injection and testability.
/// </remarks>


using RestaurantSolution.Model.Entities;

namespace RestaurantSolution.Model.Repositories
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        User GetUserByUsername(string username);
        bool InsertUser(User user);
        bool UpdateUser(User user);
        bool UpdateUserPassword(int userId, string passwordHash);
        bool DeleteUser(int id);
        bool UsernameExists(string username);
        bool EmailExists(string email);
    }
}