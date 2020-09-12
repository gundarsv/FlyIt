namespace FlyIt.Services.Models
{
    public class AuthenticationToken
    {
        public string AccessToken { get; set; }

        public long ExpiresIn { get; set; }

        public string RefreshToken { get; set; }
    }
}
