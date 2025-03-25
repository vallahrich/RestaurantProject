using Microsoft.AspNetCore.Mvc;
using RestaurantSolution.Model.Entities;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreference : ControllerBase
    {
        private readonly UserPreferenceRepository _userPreferenceRepository;
        private readonly UserRepository _userRepository;

        public UserPreference(UserPreferenceRepository userPreferenceRepository, UserRepository userRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
            _userRepository = userRepository;
        }

        // GET: api/userpreference/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Model.Entities.UserPreference> GetByUserId(int userId)
        {
            // Verify user exists
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            var preference = _userPreferenceRepository.GetByUserId(userId);
            if (preference == null)
            {
                return NotFound($"Preferences for user with ID {userId} not found");
            }
            
            return Ok(preference);
        }

        // GET: api/userpreference/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Model.Entities.UserPreference> GetById(int id)
        {
            var preference = _userPreferenceRepository.GetById(id);
            if (preference == null)
            {
                return NotFound($"Preference with ID {id} not found");
            }
            
            return Ok(preference);
        }

        // POST: api/userpreference
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<Model.Entities.UserPreference> CreateUserPreference(Model.Entities.UserPreference preference)
        {
            // Verify user exists
            var user = _userRepository.GetUserById(preference.userId);
            if (user == null)
            {
                return NotFound($"User with ID {preference.userId} not found");
            }

            // Check if user already has preferences
            var existingPreference = _userPreferenceRepository.GetByUserId(preference.userId);
            if (existingPreference != null)
            {
                return Conflict($"User with ID {preference.userId} already has preferences");
            }

            bool success = _userPreferenceRepository.InsertUserPreference(preference);
            if (!success)
            {
                return BadRequest("Failed to create user preferences");
            }

            return CreatedAtAction(nameof(GetById), new { id = preference.preferenceId }, preference);
        }

        // PUT: api/userpreference
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateUserPreference(Model.Entities.UserPreference preference)
        {
            // Check if preference exists
            var existingPreference = _userPreferenceRepository.GetById(preference.preferenceId);
            if (existingPreference == null)
            {
                return NotFound($"Preference with ID {preference.preferenceId} not found");
            }

            bool success = _userPreferenceRepository.UpdateUserPreference(preference);
            if (!success)
            {
                return BadRequest("Failed to update user preferences");
            }

            return Ok(preference);
        }

        // DELETE: api/userpreference/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteUserPreference(int id)
        {
            // Check if preference exists
            var existingPreference = _userPreferenceRepository.GetById(id);
            if (existingPreference == null)
            {
                return NotFound($"Preference with ID {id} not found");
            }

            bool success = _userPreferenceRepository.DeleteUserPreference(id);
            if (!success)
            {
                return BadRequest("Failed to delete user preferences");
            }

            return NoContent();
        }
    }
}