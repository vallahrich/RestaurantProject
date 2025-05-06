/// <summary>
/// Controller that handles user management API endpoints including authentication, registration,
/// profile updates, and account management.
/// </summary>
/// <remarks>
/// Provides RESTful API endpoints for:
/// - User retrieval by ID (GET)
/// - User authentication (login) with token generation
/// - User registration with duplicate validation
/// - Username updates with conflict checking
/// - Password changes with verification
/// - Account deletion
/// 
/// All endpoints use appropriate HTTP status codes and response types for RESTful compliance.
/// Most endpoints require authentication except those marked with [AllowAnonymous].
/// The controller follows standard REST conventions and includes validation to ensure
/// secure user management operations.
/// </remarks>

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;
using RestaurantSolution.API.Middleware;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase //from the Microsoft.AspNetCore.Mvc namespace
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id); //without FromRoute: default behavior for route parameters
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }
            return Ok(user);
        }

        // POST: api/user/login
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult Login(LoginRequest request)
        {
            var user = _userRepository.GetUserByUsername(request.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            if (user.PasswordHash != request.PasswordHash)
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate authentication header using AuthenticationHelper
            var headerValue = AuthenticationHelper.Encrypt(request.Username, request.PasswordHash);

            // Return both user and header value
            return Ok(new { user = user, headerValue = headerValue });
        }

        // POST: api/user/register
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<User> Register(User user)
        {
            // Check if username or email already exists
            if (_userRepository.UsernameExists(user.Username))
            {
                return Conflict($"Username '{user.Username}' already exists");
            }

            if (_userRepository.EmailExists(user.Email))
            {
                return Conflict($"Email '{user.Email}' already exists");
            }

            bool success = _userRepository.InsertUser(user);
            if (!success)
            {
                return BadRequest("Failed to create user");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        // PUT: api/user
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult UpdateUser(User user)
        {
            // Check if user exists
            var existingUser = _userRepository.GetUserById(user.UserId);
            if (existingUser == null)
            {
                return NotFound($"User with ID {user.UserId} not found");
            }

            // Check if the new username is already taken by another user
            if (user.Username != existingUser.Username)
            {
                var userWithSameUsername = _userRepository.GetUserByUsername(user.Username);
                if (userWithSameUsername != null && userWithSameUsername.UserId != user.UserId)
                {
                    return Conflict($"Username '{user.Username}' is already taken");
                }
            }

            // Don't allow email changes for simplicity
            user.Email = existingUser.Email;

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
        public ActionResult UpdatePassword(PasswordUpdateRequest request)
        {
            // Check if user exists
            var existingUser = _userRepository.GetUserById(request.UserId);
            if (existingUser == null)
            {
                return NotFound($"User with ID {request.UserId} not found");
            }

            // Verify old password
            if (existingUser.PasswordHash != request.OldPasswordHash)
            {
                return BadRequest("Current password is incorrect");
            }

            bool success = _userRepository.UpdateUserPassword(request.UserId, request.NewPasswordHash);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }

    public class PasswordUpdateRequest
    {
        public int UserId { get; set; }
        public string OldPasswordHash { get; set; }
        public string NewPasswordHash { get; set; }
    }
}