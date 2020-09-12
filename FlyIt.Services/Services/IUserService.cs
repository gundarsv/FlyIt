using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public interface IUserService
    {
        public Task<Result<IdentityResult>> CreateUser(User user, string password);

        public Task<Result<UserToken>> SignInUser(string userName, string password);
    }
}
