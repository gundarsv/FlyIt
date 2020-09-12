using FlyIt.DataContext;
using FlyIt.DataContext.Entities.Identity;
using System;
using System.Linq;

namespace FlyIt.Services.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly IdentityContext context;
        public UserTokenRepository(IdentityContext context)
        {
            this.context = context;
        }

        public UserToken AddUserToken(User user, string accessToken, string refreshToken, DateTime accessTokenExpiration, DateTime refreshTokenExpiration)
        {
            var authenticationToken = context.UserTokens.Add(new UserToken
            {
                UserId = user.Id,
                Value = accessToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                AccessTokenExpiration = accessTokenExpiration,
                LoginProvider = "FlyIt",
                RefreshToken = refreshToken,
                Name = "AuthenticationToken"
            });

            context.SaveChanges();

            return authenticationToken.Entity;
        }

        public void RemoveUserToken(User user)
        {
            var userToken = context.UserTokens.Where(token => token.UserId == user.Id).FirstOrDefault();

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
            var userToken = context.UserTokens.Where(token => token.LoginProvider == "FlyIt" && token.UserId == user.Id && token.Name == "AuthenticationToken").FirstOrDefault();
            
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
