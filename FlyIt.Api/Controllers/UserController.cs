using AutoMapper;
using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Api.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using Entity = FlyIt.DataAccess.Entities.Identity;

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

        [AuthorizeRoles(Roles.SystemAdministrator)]
        [HttpGet("Users")]
        public async Task<ActionResult> GetUsers()
        {
            var result = await userService.GetUsers();

            return this.FromResult(result);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
