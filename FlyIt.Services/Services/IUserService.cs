using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IUserService
    {
        public Task<Result<UserDTO>> GetUser(ClaimsPrincipal claims);

        public Task<Result<IdentityResult>> CreateUser(string email, string fullName, string password);

        public Task<Result<AuthenticationToken>> SignInUser(string email, string password);
    }
}
