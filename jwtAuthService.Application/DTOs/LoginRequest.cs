namespace jwtAuthService.Application.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DeviceFingerprint { get; set; } = string.Empty;
    }
}
