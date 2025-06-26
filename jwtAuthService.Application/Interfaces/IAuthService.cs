using jwtAuthService.Application.DTOs;

namespace jwtAuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string origin);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(string refreshToken, string deviceFingerprint);
        Task ConfirmEmailAsync(string userId, string token);
    }
}
