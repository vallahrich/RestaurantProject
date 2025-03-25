using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class User : ControllerBase
{
    private readonly UserRepository _userRepository;

    public User(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/user
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Model.Entities.User>> GetUsers()
    {
        var users = _userRepository.GetUsers();
        return Ok(users);
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Model.Entities.User> GetUserById(int id)
    {
        var user = _userRepository.GetUserById(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found");
        }
        return Ok(user);
    }

    // GET: api/user/byusername/{username}
    [HttpGet("byusername/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Model.Entities.User> GetUserByUsername(string username)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            return NotFound($"User with username '{username}' not found");
        }
        return Ok(user);
    }

    // POST: api/user
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Model.Entities.User> CreateUser(Model.Entities.User user)
    {
        // Check if username or email already exists
        if (_userRepository.UsernameExists(user.username))
        {
            return Conflict($"Username '{user.username}' already exists");
        }

        if (_userRepository.EmailExists(user.email))
        {
            return Conflict($"Email '{user.email}' already exists");
        }

        bool success = _userRepository.InsertUser(user);
        if (!success)
        {
            return BadRequest("Failed to create user");
        }

        return CreatedAtAction(nameof(GetUserById), new { id = user.userId }, user);
    }

    // PUT: api/user
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult UpdateUser(Model.Entities.User user)
    {
        // Check if user exists
        var existingUser = _userRepository.GetUserById(user.userId);
        if (existingUser == null)
        {
            return NotFound($"User with ID {user.userId} not found");
        }

        // Check if the new username is already taken by another user
        var userWithSameUsername = _userRepository.GetUserByUsername(user.username);
        if (userWithSameUsername != null && userWithSameUsername.userId != user.userId)
        {
            return Conflict($"Username '{user.username}' is already taken");
        }

        bool success = _userRepository.UpdateUser(user);
        if (!success)
        {
            return BadRequest("Failed to update user");
        }

        return Ok(user);
    }

    // PUT: api/user/password
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult UpdatePassword(int userId, string passwordHash)
    {
        // Check if user exists
        var existingUser = _userRepository.GetUserById(userId);
        if (existingUser == null)
        {
            return NotFound($"User with ID {userId} not found");
        }

        bool success = _userRepository.UpdateUserPassword(userId, passwordHash);
        if (!success)
        {
            return BadRequest("Failed to update password");
        }

        return Ok("Password updated successfully");
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteUser(int id)
    {
        // Check if user exists
        var existingUser = _userRepository.GetUserById(id);
        if (existingUser == null)
        {
            return NotFound($"User with ID {id} not found");
        }

        bool success = _userRepository.DeleteUser(id);
        if (!success)
        {
            return BadRequest("Failed to delete user");
        }

        return NoContent();
    }

    // GET: api/user/exists/username/{username}
    [HttpGet("exists/username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> UsernameExists(string username)
    {
        return Ok(_userRepository.UsernameExists(username));
    }

    // GET: api/user/exists/email/{email}
    [HttpGet("exists/email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> EmailExists(string email)
    {
        return Ok(_userRepository.EmailExists(email));
    }
}