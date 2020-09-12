using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;

        public UserService(UserManager<User> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        public async Task<Result<IdentityResult>> CreateUser(User user, string password)
        {
            try
            {
                var userCreateResult = await userManager.CreateAsync(user, password);

                if (userCreateResult.Succeeded)
                {
                    return new CreatedResult<IdentityResult>(userCreateResult);
                }

                return new InvalidResult<IdentityResult>(userCreateResult.Errors.First().Description);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<IdentityResult>(ex.Message);
            }
            
        }

        public async Task<Result<UserToken>> SignInUser(string userName, string password)
        {
            try
            {
                var user = userManager.Users.SingleOrDefault(u => u.UserName == userName);

                if (user is null)
                {
                    return new InvalidResult<UserToken>("User not found");
                }

                var userSigninResult = await userManager.CheckPasswordAsync(user, password);

                if (!userSigninResult)
                {
                    return new InvalidResult<UserToken>("Username or password is incorrect.");
                }

                var roles = await userManager.GetRolesAsync(user);
                var result = await tokenService.GenerateAuthenticationTokenAsync(user);

                return result;
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<UserToken>(ex.Message);
            }
            
        }
    }
}
