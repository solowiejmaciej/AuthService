namespace AuthService.Models
{
    public class JwtAppSettings
    {
        public string JwtPublicKey { get; set; }
        public int JwtExpireMinutes { get; set; }
        public string JwtIssuer { get; set; }
    }
}