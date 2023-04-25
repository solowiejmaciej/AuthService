namespace AuthService.Models
{
    public class JwtAppSettings
    {
        public string JwtPublicKey { get; set; }
        public int JwtExpireDays { get; set; }
        public string JwtIssuer { get; set; }
    }
}