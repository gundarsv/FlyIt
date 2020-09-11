using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FlyIt.Api.Attributes;
using FlyIt.Api.Models;
using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("Roles")]
        public async Task<IActionResult> CreateRole([Required] string roleName)
        {
            var result = await roleService.CreateRole(roleName);

            return result;
        }

        [HttpPost("User/{userEmail}/Role")]
        public async Task<IActionResult> AddUserToRole([Required] string userEmail, [Required, FromQuery] string roleName)
        {
            var result = await roleService.AddRole(userEmail, roleName);

            return result;
        }
    }
}
