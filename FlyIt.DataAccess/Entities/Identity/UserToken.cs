using Microsoft.AspNetCore.Identity;
using System;

namespace FlyIt.DataAccess.Entities.Identity
{
    public class UserToken : IdentityUserToken<int>
    {
        public string RefreshToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
}
