using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly FlyItContext context;

        public UserTokenRepository(FlyItContext context)
        {
            this.context = context;
        }

        public async Task<UserToken> UpdateUserTokenAsync(UserToken userToken)
        {
            var userTokenToUpdate = await context.UserToken.SingleOrDefaultAsync(ut => ut.Id == userToken.Id && ut.RefreshToken == userToken.RefreshToken);

            if (userTokenToUpdate is null)
            {
                return null;
            }

            context.Entry(userTokenToUpdate).CurrentValues.SetValues(userToken);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return await context.UserToken.SingleOrDefaultAsync(ut => ut.Id == userToken.Id && ut.RefreshToken == userToken.RefreshToken);
        }

        public async Task<UserToken> AddUserTokenAsync(UserToken userToken)
        {
            if (userToken is null)
            {
                return null;
            }

            var entityEntry = await context.UserToken.AddAsync(userToken);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return entityEntry.Entity;
        }

        public async Task<UserToken> RemoveUserTokenAsync(UserToken userToken)
        {
            if (userToken is null)
            {
                return null;
            }

            var removedUserToken = context.UserToken.Remove(userToken);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return removedUserToken.Entity;
        }

        public async Task<UserToken> GetUserTokenByRefreshAndAccessTokenAsync(string refreshToken, string accessToken)
        {
            if (refreshToken is null || accessToken is null)
            {
                return null;
            }

            return await context.UserToken.SingleOrDefaultAsync(ut => ut.RefreshToken == refreshToken && ut.AccessToken == accessToken);
        }
    }
}