using Meme.Hub.Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Meme.Hub.Site.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> GetTokenAsync(GetOrRegisterUserRequestDto request);

        Task<AuthResponseDto?> RefreshTokenAsync(string oldRefreshToken);
    }
    public class AuthService: IAuthService
    {
        private readonly DataStore _dataStore;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient; // For fetching JWKS
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private List<SecurityKey>? _privySigningKeys; // Cache Privy's public keys
        private DateTime _lastJwksFetchTime = DateTime.MinValue;
        private readonly TimeSpan _jwksCacheDuration = TimeSpan.FromHours(6); // Cache duration

        public AuthService(DataStore dataStore, IConfiguration configuration)
        {
            _dataStore = dataStore;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        private async Task<bool> VerifyPrivyAccessToken(string privyAccessToken, string expectedPrivyId)
        {
            // 1. Fetch and Cache Privy's JWKS
            // Replace with the actual Privy JWKS URL. You can typically find this in Privy's docs
            // or by looking for a `.well-known/jwks.json` endpoint related to your Privy app.
            // Example: "https://your-privy-app-id.privy.io/.well-known/jwks.json" or similar.
            // Check Privy's documentation for the exact JWKS endpoint.
            /*
             var privyJwksUrl = _configuration["Privy:JwksUrl"] ??
                                throw new InvalidOperationException("Privy JWKS URL is not configured.");

            if (_privySigningKeys == null || (DateTime.UtcNow - _lastJwksFetchTime) > _jwksCacheDuration)
            {
                try
                {
                    var jwksResponse = await _httpClient.GetStringAsync(privyJwksUrl);
                    var jwks = new JsonWebKeySet(jwksResponse);
                    _privySigningKeys = jwks.Keys.Select(k => (SecurityKey)k).ToList();
                    _lastJwksFetchTime = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching Privy JWKS: {ex.Message}");
                    // If JWKS cannot be fetched, we cannot validate, so treat as invalid.
                    return false;
                }
            }

            if (_privySigningKeys == null || !_privySigningKeys.Any())
            {
                Console.WriteLine("No Privy signing keys available.");
                return false;
            }*/

            // 2. Define Token Validation Parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Privy:SigningKey"]!)), // Use the fetched keys

                ValidateIssuer = true,
                ValidIssuer = _configuration["Privy:Issuer"], // Privy's issuer (e.g., "privy.io")

                ValidateAudience = true,
                ValidAudience = _configuration["Privy:Audience"], // Your Privy app ID

                ValidateLifetime = true, // Check expiration and not-before times
                ClockSkew = TimeSpan.FromMinutes(5) // Allow for slight clock differences (e.g., 5 minutes)
            };

            try
            {
                // 3. Validate the token
                SecurityToken validatedToken;
                var principal = _jwtSecurityTokenHandler.ValidateToken(
                    privyAccessToken,
                    tokenValidationParameters,
                    out validatedToken
                );

                // 4. Verify Subject (Privy ID) matches the expected ID from the request
                var privyIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (privyIdClaim != expectedPrivyId)
                {
                    Console.WriteLine($"Privy ID mismatch: Expected {expectedPrivyId}, Got {privyIdClaim}");
                    return false;
                }

                Console.WriteLine($"Privy access token for {expectedPrivyId} successfully validated.");
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Privy access token expired.");
                return false;
            }
            catch (SecurityTokenValidationException ex)
            {
                Console.WriteLine($"Privy access token validation failed: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during Privy token validation: {ex.Message}");
                return false;
            }
        }

        public async Task<AuthResponseDto?> GetTokenAsync(GetOrRegisterUserRequestDto request)
        {
            var isPrivyTokenValid = await VerifyPrivyAccessToken(request.PrivyAccessToken, request.PrivyId);
            if (!isPrivyTokenValid)
            {
                throw new UnauthorizedAccessException("Invalid or expired Privy access token.");
            }

            var user = _dataStore.Users.FirstOrDefault(u => u.PrivyId == request.PrivyId);
            if (user == null)
            {
                user = new User
                {
                    PrivyId = request.PrivyId,
                    Username = request.Username ?? $"user_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    Email = request.Email ?? $"{request.PrivyId}@privy.io",
                    ProfileImage = "https://i.pravatar.cc/150?img=default",
                    SocialLinks = new SocialLinks(),
                    Settings = new UserSettings()
                };
                _dataStore.Users.Add(user);
                Console.WriteLine($"New user registered: {user.Username} ({user.PrivyId})");
            }
            else
            {
                Console.WriteLine($"Existing user logged in: {user.Username} ({user.PrivyId})");
                // Optionally update user details from Privy if they've changed
                if (!string.IsNullOrEmpty(request.Username) && user.Username != request.Username) user.Username = request.Username;
                if (!string.IsNullOrEmpty(request.Email) && user.Email != request.Email) user.Email = request.Email;
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user
            };
        }

        private string GenerateAccessToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user._id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("privyId", user.PrivyId),
                new Claim("username", user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Key"]; // Use the same key or a different one for refresh tokens
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user._id), // Subject claim: your internal user ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique ID for this refresh token
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Same issuer as access token or distinct
                audience: _configuration["Jwt:Audience"], // Same audience as access token or distinct
                claims: claims,
                // Set expiration to 1 year
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"])),
                signingCredentials: credentials);

            return _jwtSecurityTokenHandler.WriteToken(token);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string oldRefreshToken)
        {
            var jwtSecret = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5) // Allow for slight clock differences
            };

            ClaimsPrincipal principal;
            try
            {
                // Validate the refresh token JWT
                principal = _jwtSecurityTokenHandler.ValidateToken(
                    oldRefreshToken,
                    tokenValidationParameters,
                    out SecurityToken validatedToken
                );
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedAccessException("Refresh token expired. Please log in again.");
            }
            catch (SecurityTokenValidationException ex)
            {
                throw new UnauthorizedAccessException($"Invalid refresh token: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating refresh token: {ex.Message}");
                throw new UnauthorizedAccessException("An unexpected error occurred during refresh token validation.");
            }

            // Get the user ID from the refresh token's subject claim
            var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid refresh token claims.");
            }

            var user = _dataStore.Users.FirstOrDefault(u => u._id == userId);

            // IMPORTANT: Implement refresh token rotation and revocation check here.
            // This is a crucial security step for refresh tokens.
            // 1. Check if the refresh token stored in the database matches the 'oldRefreshToken'.
            //    This prevents multiple uses of the same refresh token.
            // 2. If it matches, invalidate the old refresh token in the database (e.g., set to null or blacklist).
            //    If it does NOT match, it could indicate a token reuse attack. You should revoke all refresh tokens for this user.
            if (user == null || user.RefreshToken != oldRefreshToken)
            {
                // Optionally: If user is found but refresh token doesn't match,
                // consider revoking all user tokens (if you track multiple per user)
                // or forcing re-login, as this could indicate compromise.
                throw new UnauthorizedAccessException("Invalid or revoked refresh token.");
            }

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken(user); // Generate a new JWT refresh token
            user.RefreshToken = newRefreshToken; // Store the new refresh token

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                User = user
            };
        }
    }
}
