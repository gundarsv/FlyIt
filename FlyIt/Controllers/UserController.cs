using AutoMapper;
using FlyIt.Api.Extensions;
using FlyIt.Api.Models;
using FlyIt.Services.ServiceResult;
using FlyIt.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using Entity = FlyIt.DataContext.Entities.Identity;

namespace FlyIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await userService.GetUser(User);

            if (result.Data == null)
            {
                return this.FromResult(result);
            }

            var user = mapper.Map<Entity.User, User>(result.Data);

            return this.FromResult(new SuccessResult<User>(user));
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
