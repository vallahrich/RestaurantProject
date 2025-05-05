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