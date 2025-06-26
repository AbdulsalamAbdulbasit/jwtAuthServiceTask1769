namespace jwtAuthService.Application.DTOs
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string DeviceFingerprint { get; set; } = string.Empty;
    }
}
