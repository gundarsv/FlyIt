using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface ITokenService
    {
        public Task<Result<AuthenticationToken>> RefreshTokenAsync(string refreshToken, string accessToken);

        public Task<Result<AuthenticationToken>> GenerateAuthenticationTokenAsync(User user, string loginProvider, AuthenticationFlow authenticationFlow);
    }
}