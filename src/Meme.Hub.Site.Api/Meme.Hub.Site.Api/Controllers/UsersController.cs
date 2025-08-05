using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;
using Meme.Hub.Site.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // e.g., /api/users
    public class UsersController : ControllerBase
    {
        private readonly DataStore _dataStore;
        private readonly IUserService _userService;

        public UsersController(DataStore dataStore, IUserService userService)
        {
            _dataStore = dataStore;
            _userService = userService;
        }

        // GET: api/users/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(string userId)
        {
            if(string.IsNullOrEmpty(userId)) return BadRequest(ModelState);

            var user = await _userService.GetUserByPrivyId(userId); // _dataStore.Users.FirstOrDefault(u => u._id == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        // GET: api/users/privy/{privyId} - Used for initial user check/creation
        [HttpGet("privy/{privyId}")]
        public ActionResult<User> GetUserByPrivyId(string privyId)
        {
            var user = _dataStore.Users.FirstOrDefault(u => u.PrivyId == privyId);
            if (user == null)
            {
                // In a real app, you might auto-create a user here or redirect to onboarding
                return NotFound("User not registered with this Privy ID.");
            }
            return Ok(user);
        }

        // POST: api/users/register
        // This is a simplified registration. In a real app, Privy.io handles
        // most of the core auth, this endpoint would just create your internal user record.
        [HttpPost("register")]
        public ActionResult<User> RegisterUser([FromBody] User newUser)
        {
            if (string.IsNullOrEmpty(newUser.PrivyId) || string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Email))
            {
                return BadRequest("PrivyId, Username, and Email are required for registration.");
            }

            // Check if user already exists (by PrivyId or username/email)
            if (_dataStore.Users.Any(u => u.PrivyId == newUser.PrivyId || u.Username == newUser.Username || u.Email == newUser.Email))
            {
                return Conflict("User with this Privy ID, username, or email already exists.");
            }

            // Set default values
            newUser._id = Guid.NewGuid().ToString();
            newUser.CreatedAt = DateTime.UtcNow; // Assuming you add CreatedAt to User model if not already

            _dataStore.Users.Add(newUser);
            return CreatedAtAction(nameof(GetUser), new { userId = newUser._id }, newUser);
        }

        // PATCH: api/users/{userId} - Update user profile
        [HttpPatch("{userId}")]
        public ActionResult<User> UpdateUser(string userId, [FromBody] User updatedUser)
        {
            var user = _dataStore.Users.FirstOrDefault(u => u._id == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Apply updates (only allow specific fields to be updated)
            if (!string.IsNullOrEmpty(updatedUser.Username)) user.Username = updatedUser.Username;
            if (!string.IsNullOrEmpty(updatedUser.Email)) user.Email = updatedUser.Email;
            if (!string.IsNullOrEmpty(updatedUser.Bio)) user.Bio = updatedUser.Bio;
            if (!string.IsNullOrEmpty(updatedUser.ProfileImage)) user.ProfileImage = updatedUser.ProfileImage;
            if (updatedUser.SocialLinks != null) user.SocialLinks = updatedUser.SocialLinks;
            if (updatedUser.Settings != null) user.Settings = updatedUser.Settings;


            // You might want to handle unique constraints for username/email if updated
            // For simplicity, we're not doing that here.

            return Ok(user);
        }

        // POST: api/users/{followerId}/follow/{followeeId}
        [HttpPost("{followerId}/follow/{followeeId}")]
        public IActionResult FollowUser(string followerId, string followeeId)
        {
            var follower = _dataStore.Users.FirstOrDefault(u => u._id == followerId);
            var followee = _dataStore.Users.FirstOrDefault(u => u._id == followeeId);

            if (follower == null || followee == null)
            {
                return NotFound("Follower or followee not found.");
            }
            if (followerId == followeeId)
            {
                return BadRequest("Cannot follow yourself.");
            }

            if (!follower.Following.Contains(followeeId))
            {
                follower.Following.Add(followeeId);
            }
            if (!followee.Followers.Contains(followerId))
            {
                followee.Followers.Add(followerId);
            }

            return NoContent(); // 204 No Content
        }

        // POST: api/users/{followerId}/unfollow/{followeeId}
        [HttpPost("{followerId}/unfollow/{followeeId}")]
        public IActionResult UnfollowUser(string followerId, string followeeId)
        {
            var follower = _dataStore.Users.FirstOrDefault(u => u._id == followerId);
            var followee = _dataStore.Users.FirstOrDefault(u => u._id == followeeId);

            if (follower == null || followee == null)
            {
                return NotFound("Follower or followee not found.");
            }

            follower.Following.Remove(followeeId);
            followee.Followers.Remove(followerId);

            return NoContent();
        }
    }
}
