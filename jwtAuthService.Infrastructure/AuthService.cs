using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwtAuthService.Application.DTOs;
using jwtAuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using jwtAuthService.Application.Interfaces;
using jwtAuthService.Domain.Model;

namespace jwtAuthService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<IdentityUser<Guid>> userManager,
            SignInManager<IdentityUser<Guid>> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string origin)
        {
            try
            {
                var user = new IdentityUser<Guid>
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    EmailConfirmed = false
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                }
                // Assign default role
                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "User" });
                await _userManager.AddToRoleAsync(user, "User");
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // TODO: Send confirmation email with token (implement email sender)
                return new AuthResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    AccessTokenExpires = DateTime.MinValue,
                    RefreshTokenExpires = DateTime.MinValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", request.Email);
                throw;
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                    throw new Exception("Invalid credentials");
                if (!user.EmailConfirmed)
                    throw new Exception("Email not confirmed");
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                    throw new Exception("Invalid credentials");
                // Generate JWT and refresh token
                var accessToken = GenerateJwtToken(user);
                var refreshToken = Guid.NewGuid().ToString(); // TODO: Store securely and associate with user/device
                return new AuthResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpires = DateTime.UtcNow.AddMinutes(5),
                    RefreshTokenExpires = DateTime.UtcNow.AddDays(7)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.Email);
                throw;
            }
        }

        private string GenerateJwtToken(IdentityUser<Guid> user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _dbContext.RefreshTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.DeviceFingerprint == request.DeviceFingerprint);
            if (refreshToken == null || !refreshToken.IsActive)
                throw new Exception("Invalid or expired refresh token");
            // Revoke old token
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = "TODO: Get IP from context";
            // Generate new tokens
            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();
            var newTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = "TODO: Get IP from context",
                UserId = user.Id,
                DeviceFingerprint = request.DeviceFingerprint
            };
            _dbContext.RefreshTokens.Add(newTokenEntity);
            await _dbContext.SaveChangesAsync();
            return new AuthResponse
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpires = DateTime.UtcNow.AddMinutes(5),
                RefreshTokenExpires = newTokenEntity.Expires
            };
        }

        public async Task LogoutAsync(string refreshToken, string deviceFingerprint)
        {
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.DeviceFingerprint == deviceFingerprint);
            if (token != null && token.IsActive)
            {
                token.Revoked = DateTime.UtcNow;
                token.RevokedByIp = "TODO: Get IP from context";
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new Exception("Email confirmation failed");
        }
    }
}

// Ensure the Infrastructure project references the Application project in your .csproj file:
// <ProjectReference Include="..\jwtAuthService.Application\jwtAuthService.Application.csproj" />
