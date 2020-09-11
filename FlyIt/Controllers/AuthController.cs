using AutoMapper;
using FlyIt.Api.Models;
using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FlyIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AuthController(IUserService userService, IMapper mapper)
        {
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUpAsync(SignUp model)
        {
            var user = mapper.Map<SignUp, User>(model);

            var result = await userService.CreateUser(user, model.Password);
            
            return result;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([Required] string userName, [Required] string password)
        {
            var result = await userService.SignInUser(userName, password);

            return result;
        }
    }
}
