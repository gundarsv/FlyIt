using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IUserTokenRepository
    {
        public Task<UserToken> RemoveUserTokenAsync(UserToken userToken);

        public Task<UserToken> AddUserTokenAsync(UserToken userToken);

        public Task<UserToken> UpdateUserTokenAsync(UserToken userToken);

        public Task<UserToken> GetUserTokenByRefreshAndAccessTokenAsync(string refreshToken, string accessToken);
    }
}