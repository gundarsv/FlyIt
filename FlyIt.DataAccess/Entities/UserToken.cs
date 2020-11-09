using FlyIt.DataAccess.Entities.Identity;
using System;

namespace FlyIt.DataAccess.Entities
{
    public class UserToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string LoginProvider { get; set; }

        public DateTime AccessTokenExpiration { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }

        public int AuthenticationFlow { get; set; }
    }
}