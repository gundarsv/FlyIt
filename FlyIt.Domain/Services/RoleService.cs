using FlyIt.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> AddRole(string email, string roleName)
        {
            var user = userManager.Users.SingleOrDefault(u => u.UserName == email);

            if (user is null)
            {
                return new NotFoundObjectResult(email);
            }

            var result = await userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return new OkResult();
            }

            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleResult = await roleManager.CreateAsync(new Role
            {
                Name = roleName
            });

            if (roleResult.Succeeded)
            {
                return new OkResult();
            }

            return new BadRequestObjectResult(roleResult.Errors);
        }
    }
}
