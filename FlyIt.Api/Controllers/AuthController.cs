using FlyIt.Api.Extensions;
using FlyIt.Api.Models;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlyIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(SignUp signUp)
        {
            var result = await userService.CreateUser(signUp.Email, signUp.FullName, signUp.Password);
            
            return this.FromResult(result);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignIn signIn)
        {
            var result = await userService.SignInUser(signIn.Email, signIn.Password);

            return this.FromResult(result);
        }

        [HttpPost("Revoke")]
        public async Task<IActionResult> RefreshToken(TokenRefresh tokenRefresh)
        {
            var result = await tokenService.RefreshTokenAsync(tokenRefresh.RefreshToken, tokenRefresh.AccessToken);

            return this.FromResult(result);
        }

        [HttpPost("sysadmin/SignIn")]
        public async Task<IActionResult> SignInSystemAdministrator(SignIn signIn)
        {
            var result = await userService.SignInSystemAdministrator(signIn.Email, signIn.Password);

            return this.FromResult(result);
        }

        [HttpPost("airadmin/SignIn")]
        public async Task<IActionResult> SignInAirportsAdministrator(SignIn signIn)
        {
            var result = await userService.SignInAirportsAdministrator(signIn.Email, signIn.Password);

            return this.FromResult(result);
        }
    }
}
