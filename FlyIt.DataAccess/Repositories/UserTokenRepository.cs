using FlyIt.DataAccess;
using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Linq;

namespace FlyIt.DataAccess.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly FlyItContext context;
        public UserTokenRepository(FlyItContext context)
        {
            this.context = context;
        }

        public UserToken AddUserToken(User user, string accessToken, string refreshToken, DateTime accessTokenExpiration, DateTime refreshTokenExpiration, string loginProvider)
        {
            var authenticationToken = context.UserTokens.Add(new UserToken
            {
                UserId = user.Id,
                Value = accessToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                AccessTokenExpiration = accessTokenExpiration,
                LoginProvider = loginProvider,
                RefreshToken = refreshToken,
                Name = "AuthenticationToken"
            });

            context.SaveChanges();

            return authenticationToken.Entity;
        }

        public void RemoveUserToken(User user, string loginProvider)
        {
            var userToken = context.UserTokens.SingleOrDefault(token => token.UserId == user.Id && token.LoginProvider == loginProvider);

            if (userToken != null)
            {
                context.UserTokens.Remove(userToken);

                context.SaveChanges();
            }
        }

        public UserToken UpdateUserToken(User user, string accessToken, DateTime accessTokenExpiration)
        {
            var userToken = context.UserTokens.Where(token => token.UserId == user.Id).FirstOrDefault();

            if (userToken == null)
            {
                return userToken;
            }

            userToken.Value = accessToken;

            if (accessTokenExpiration >= userToken.RefreshTokenExpiration)
            {
                userToken.AccessTokenExpiration = userToken.RefreshTokenExpiration;
            } 
            else {
                userToken.AccessTokenExpiration = accessTokenExpiration;
            }

            context.SaveChangesAsync();
            
            return userToken;
        }

        public bool ValidateAuthenticationToken(User user, string refreshToken, string accessToken)
        {
            var userToken = context.UserTokens.SingleOrDefault(token => token.LoginProvider == "FlyIt" && token.UserId == user.Id && token.Name == "AuthenticationToken");
            
            if (userToken is null)
            {
                return false;
            }

            if ((userToken.Value != accessToken) || (userToken.RefreshToken != refreshToken))
            {
                return false;
            }

            if (userToken.RefreshTokenExpiration <= DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}
