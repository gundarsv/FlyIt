using System;

namespace FlyIt.Services.Models
{
    public class AuthenticationToken
    {
        public string AccessToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string RefreshToken { get; set; }
    }
}
