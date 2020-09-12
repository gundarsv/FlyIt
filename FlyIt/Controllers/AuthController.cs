using AutoMapper;
using FlyIt.Api.Extensions;
using FlyIt.Api.Models;
using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.Models;
using FlyIt.Services.ServiceResult;
using FlyIt.Services.Services;
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
        private readonly IMapper mapper;

        public AuthController(IUserService userService, ITokenService tokenService, IMapper mapper)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.tokenService = tokenService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(SignUp signUp)
        {
            var user = mapper.Map<SignUp, User>(signUp);

            var result = await userService.CreateUser(user, signUp.Password);
            
            return this.FromResult(result);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignIn signIn)
        {
            var result = await userService.SignInUser(signIn.UserName, signIn.Password);

            if (result.Data == null)
            {
                return this.FromResult(result);
            }

            var authenticationToken = mapper.Map<UserToken, AuthenticationToken>(result.Data);

            return this.FromResult(new SuccessResult<AuthenticationToken>(authenticationToken));
        }

        [HttpPost("Revoke")]
        public async Task<IActionResult> RefreshToken(TokenRefresh tokenRefresh)
        {
            var result = await tokenService.RefreshTokenAsync(tokenRefresh.RefreshToken, tokenRefresh.AccessToken);

            if (result.Data == null)
            {
                return this.FromResult(result);
            }

            var authenticationToken = mapper.Map<UserToken, AuthenticationToken>(result.Data);

            return this.FromResult(new SuccessResult<AuthenticationToken>(authenticationToken));
        }
    }
}
