using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.ServiceResult;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public interface ITokenService
    {
        public Task<Result<UserToken>> RefreshTokenAsync(string refreshToken, string accessToken);

        public Task<Result<UserToken>> GenerateAuthenticationTokenAsync(User user);
    }
}
