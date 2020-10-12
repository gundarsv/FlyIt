using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FlyIt.Api.Controllers
{
    [AuthorizeRoles(Roles.SystemAdministrator)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await roleService.GetRoles();

            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([Required] string roleName)
        {
            var result = await roleService.CreateRole(roleName);

            return this.FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await roleService.DeleteRole(id);

            return this.FromResult(result);
        }

        [HttpPost("User/{userId}/Role/{roleId}")]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleId)
        {
            var result = await roleService.AddRole(userId, roleId);

            return this.FromResult(result);
        }

        [HttpDelete("User/{userId}/Role/{roleId}")]
        public async Task<IActionResult> DeleteRoleFromUser(string userId, string roleId)
        {
            var result = await roleService.RemoveRole(userId, roleId);

            return this.FromResult(result);
        }
    }
}