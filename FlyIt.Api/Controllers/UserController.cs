using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlyIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await userService.GetUser(User);

            return this.FromResult(result);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            var result = await userService.DeleteUser(User);

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.SystemAdministrator)]
        [HttpGet("Users")]
        public async Task<ActionResult> GetUsers()
        {
            var result = await userService.GetUsers();

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.SystemAdministrator)]
        [HttpGet("AirportsAdministrators")]
        public async Task<ActionResult> GetAirportsAdministrators()
        {
            var result = await userService.GetAiportsAdministrators();

            return this.FromResult(result);
        }
    }
}