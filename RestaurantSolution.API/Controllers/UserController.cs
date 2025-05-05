using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;
using RestaurantSolution.API.Middleware;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/user/{id}
        // This endpoint requires authentication
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

        // POST: api/user/login
        // This endpoint is accessible without authentication
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

            // In a real app, you'd use a proper password verification method
            // For simplicity, we're just comparing the hashes directly
            if (user.passwordHash != request.PasswordHash)
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate authentication header using AuthenticationHelper
            var headerValue = AuthenticationHelper.Encrypt(request.Username, request.PasswordHash);

            // Return both user and header value
            return Ok(new { user = user, headerValue = headerValue });
        }

        // POST: api/user/register
        // This endpoint is accessible without authentication
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<Model.Entities.User> Register(Model.Entities.User user)
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
        // This endpoint requires authentication
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult UpdateUser(Model.Entities.User user)
        {
            // Check if user exists
            var existingUser = _userRepository.GetUserById(user.userId);
            if (existingUser == null)
            {
                return NotFound($"User with ID {user.userId} not found");
            }

            // Check if the new username is already taken by another user
            if (user.username != existingUser.username)
            {
                var userWithSameUsername = _userRepository.GetUserByUsername(user.username);
                if (userWithSameUsername != null && userWithSameUsername.userId != user.userId)
                {
                    return Conflict($"Username '{user.username}' is already taken");
                }
            }

            // Don't allow email changes for simplicity
            user.email = existingUser.email;

            bool success = _userRepository.UpdateUser(user);
            if (!success)
            {
                return BadRequest("Failed to update user");
            }

            return Ok(user);
        }

        // PUT: api/user/password
        // This endpoint requires authentication
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
            if (existingUser.passwordHash != request.OldPasswordHash)
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
        // This endpoint requires authentication
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