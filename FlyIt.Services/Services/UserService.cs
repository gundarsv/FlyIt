using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public UserService(UserManager<User> userManager, ITokenService tokenService, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        public async Task<Result<UserDTO>> GetUser(ClaimsPrincipal claims)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user == null)
                {
                    return new InvalidResult<UserDTO>("User not found");
                }

                var result = mapper.Map<User, UserDTO>(user);

                return new SuccessResult<UserDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<UserDTO>(ex.Message);
            }
        }

        public async Task<Result<IdentityResult>> CreateUser(string email, string fullName, string password)
        {
            try
            {
                var userDTO = new UserDTO()
                {
                    FullName = fullName,
                    Email = email,
                };

                var user = mapper.Map<UserDTO, User>(userDTO);

                var userCreateResult = await userManager.CreateAsync(user, password);

                if (userCreateResult.Succeeded)
                {
                    return new CreatedResult<IdentityResult>(userCreateResult);
                }

                return new InvalidResult<IdentityResult>(userCreateResult.Errors.First().Description);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<IdentityResult>(ex.Message);
            }
        }

        public async Task<Result<AuthenticationToken>> SignInUser(string email, string password)
        {
            try
            {
                var user = userManager.Users.SingleOrDefault(u => u.Email == email);

                if (user is null)
                {
                    return new InvalidResult<AuthenticationToken>("User not found");
                }

                var userSigninResult = await userManager.CheckPasswordAsync(user, password);

                if (!userSigninResult)
                {
                    return new InvalidResult<AuthenticationToken>("Username or password is incorrect.");
                }

                var roles = await userManager.GetRolesAsync(user);
                var result = await tokenService.GenerateAuthenticationTokenAsync(user);

                return result;
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AuthenticationToken>(ex.Message);
            }
        }
    }
}
