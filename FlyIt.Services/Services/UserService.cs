using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly JWTHelper helper;

        public UserService(UserManager<User> userManager, JWTHelper helper)
        {
            this.userManager = userManager;
            this.helper = helper;
        }

        public async Task<IActionResult> CreateUser(User user, string password)
        {
            var userCreateResult = await userManager.CreateAsync(user, password);

            if (userCreateResult.Succeeded)
            {
                return new CreatedResult(userCreateResult.ToString(), user.UserName);
            }

            return new BadRequestObjectResult(userCreateResult.Errors.First().Description);
        }

        public async Task<IActionResult> SignInUser(string userName, string password)
        {
            var user = userManager.Users.SingleOrDefault(u => u.UserName == userName);

            if (user is null)
            {
                return new NotFoundObjectResult("User not found");
            }

            var userSigninResult = await userManager.CheckPasswordAsync(user, password);

            if (userSigninResult)
            {
                var roles = await userManager.GetRolesAsync(user);
                return new OkObjectResult(helper.GenerateJwt(user, roles));
            }

            return new BadRequestObjectResult("Email or password incorrect.");
        }
    }
}
