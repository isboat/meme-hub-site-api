using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route: /api/auth/
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: /api/auth/gettoken
        [HttpPost("gettoken")]
        public async Task<ActionResult<AuthResponseDto>> GetOrRegisterUser([FromBody] GetTokenRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                var response = await _authService.GetTokenAsync(request);
                if (response == null)
                {
                    // This case should ideally not happen if AuthService handles creation/retrieval
                    return StatusCode(500, "Failed to process user authentication/registration.");
                }
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // This exception is thrown by AuthService if Privy token validation fails
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrRegisterUser: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred during authentication." });
            }
        }

        // POST: /api/auth/refresh_token
        [HttpPost("refresh_token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required." });
            }

            try
            {
                var response = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing token: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred during token refresh." });
            }
        }
    }
}
