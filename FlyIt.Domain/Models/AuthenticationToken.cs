namespace FlyIt.Domain.Models
{
    public class AuthenticationToken
    {
        public string AccessToken { get; set; }

        public long ExpiresAt { get; set; }

        public string RefreshToken { get; set; }
    }
}