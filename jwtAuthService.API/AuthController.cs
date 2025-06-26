using jwtAuthService.Application.DTOs;
using jwtAuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace jwtAuthService.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register a new user", Description = "Creates a new user and sends a confirmation email.")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var origin = Request.Headers["origin"].ToString();
            var result = await _authService.RegisterAsync(request, origin);
            return Ok(new { status = 200, message = "Registration successful. Please confirm your email.", data = result });
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login", Description = "Authenticate user and return JWT + refresh token.")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(new { status = 200, message = "Login successful", data = result });
        }

        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Refresh token", Description = "Exchange a valid refresh token for a new access token.")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(new { status = 200, message = "Token refreshed", data = result });
        }

        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Logout", Description = "Invalidate refresh token and end session.")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            await _authService.LogoutAsync(request.RefreshToken, request.DeviceFingerprint);
            return Ok(new { status = 200, message = "Logged out", data = (object?)null });
        }

        [HttpGet("confirm-email")]
        [SwaggerOperation(Summary = "Confirm email", Description = "Confirm a user's email address.")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            await _authService.ConfirmEmailAsync(userId, token);
            return Ok(new { status = 200, message = "Email confirmed", data = (object?)null });
        }
    }
}
