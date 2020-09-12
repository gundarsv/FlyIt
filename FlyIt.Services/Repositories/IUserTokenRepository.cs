using FlyIt.DataContext.Entities.Identity;
using System;

namespace FlyIt.Services.Repositories
{
    public interface IUserTokenRepository
    {
        public void RemoveUserToken(User user);

        public UserToken AddUserToken(User user, string accessToken, string refreshToken, DateTime accessTokenExpiration, DateTime refreshTokenExpiration);

        public UserToken UpdateUserToken(User user, string accessToken, DateTime accessTokenExpiration);

        public bool ValidateAuthenticationToken(User user, string refreshToken, string accessToken);
    }
}
