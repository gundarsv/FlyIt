using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Entity = FlyIt.DataAccess.Entities.Identity;

namespace FlyIt.Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings tokenSettings;
        private readonly IUserTokenRepository repository;
        private readonly UserManager<Entity.User> userManager;
        private readonly IMapper mapper;
        private readonly ILogger<TokenService> logger;

        public TokenService(IOptionsSnapshot<JWTSettings> tokenSettings, UserManager<Entity.User> userManager, IUserTokenRepository repository, IMapper mapper, ILogger<TokenService> logger)
        {
            this.tokenSettings = tokenSettings.Value;
            this.repository = repository;
            this.userManager = userManager;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<AuthenticationToken>> RefreshTokenAsync(string refreshToken, string accessToken)
        {
            try
            {
                var userToken = await repository.GetUserTokenByRefreshAndAccessTokenAsync(refreshToken, accessToken);

                if (userToken is null)
                {
                    return new NotFoundResult<AuthenticationToken>("Token not found");
                }

                var user = await userManager.FindByIdAsync(userToken.UserId.ToString());

                if (user is null)
                {
                    return new NotFoundResult<AuthenticationToken>("User not found");
                }

                var isTokenValid = IsAuthenticationTokenValid(userToken);

                if (!isTokenValid)
                {
                    var removeResult = await repository.RemoveUserTokenAsync(userToken);

                    if (removeResult is null)
                    {
                        return new InvalidResult<AuthenticationToken>("Could not remove expired token");
                    }

                    return new InvalidResult<AuthenticationToken>("Refresh token is expired");
                }

                var newAccessToken = await GenerateAccessToken(user, (AuthenticationFlow)userToken.AuthenticationFlow);

                if (newAccessToken is null)
                {
                    return new InvalidResult<AuthenticationToken>("Could not refresh token");
                }

                var updatedToken = await repository.UpdateUserTokenAsync(new UserToken()
                {
                    Id = userToken.Id,
                    AccessToken = newAccessToken,
                    UserId = userToken.UserId,
                    User = user,
                    AuthenticationFlow = userToken.AuthenticationFlow,
                    RefreshToken = userToken.RefreshToken,
                    LoginProvider = userToken.LoginProvider,
                    AccessTokenExpiration = DateTime.Now.AddDays(Convert.ToDouble(tokenSettings.AccessTokenExpirationInDays)),
                    RefreshTokenExpiration = userToken.RefreshTokenExpiration,
                });

                if (updatedToken is null)
                {
                    return new InvalidResult<AuthenticationToken>("Could not add new authentication token");
                }

                var result = mapper.Map<UserToken, AuthenticationToken>(updatedToken);

                return new SuccessResult<AuthenticationToken>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AuthenticationToken>(ex.Message);
            }
        }

        public async Task<Result<AuthenticationToken>> GenerateAuthenticationTokenAsync(Entity.User user, string loginProvider, AuthenticationFlow authenticationFlow)
        {
            try
            {
                var accessToken = await GenerateAccessToken(user, authenticationFlow);

                if (accessToken is null)
                {
                    return new InvalidResult<AuthenticationToken>("Could not generate access token");
                }

                var refreshToken = GenerateRefreshToken();

                if (refreshToken is null)
                {
                    return new InvalidResult<AuthenticationToken>("Could not generate access token");
                }

                var refreshTokenExpiration = DateTime.Now.AddDays(Convert.ToDouble(tokenSettings.RefreshTokenExpirationInDays));
                var accessTokenExpiration = DateTime.Now.AddDays(Convert.ToDouble(tokenSettings.AccessTokenExpirationInDays));

                var token = await repository.AddUserTokenAsync(new UserToken()
                {
                    UserId = user.Id,
                    User = user,
                    LoginProvider = loginProvider,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration,
                    AccessTokenExpiration = accessTokenExpiration,
                    AuthenticationFlow = (int)authenticationFlow,
                });

                if (token is null)
                {
                    return new InvalidResult<AuthenticationToken>("Could not save new authentication token");
                }

                var result = mapper.Map<UserToken, AuthenticationToken>(token);

                return new SuccessResult<AuthenticationToken>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AuthenticationToken>(ex.Message);
            }
        }

        private async Task<string> GenerateAccessToken(Entity.User user, AuthenticationFlow authenticationFlow)
        {
            try
            {
                var roles = await userManager.GetRolesAsync(user);

                if (roles is null)
                {
                    return null;
                }

                var claims = GenerateUserClaims(user, roles, authenticationFlow);

                if (claims is null)
                {
                    return null;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: tokenSettings.Issuer,
                    audience: tokenSettings.Issuer,
                    claims,
                    expires: DateTime.Now.AddDays(Convert.ToDouble(tokenSettings.AccessTokenExpirationInDays)),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError($"Generating access token for user: {user.Id} was not successfull", ex);
                throw;
            }
        }

        private string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Generating access token was not successfull", ex);
                throw;
            }
        }

        private IList<Claim> GenerateUserClaims(Entity.User user, IList<string> roles, AuthenticationFlow authenticationFlow)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                if (authenticationFlow.Equals(AuthenticationFlow.Full))
                {
                    var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
                    claims.AddRange(roleClaims);
                }

                return claims;
            }
            catch (Exception ex)
            {
                logger.LogError($"Claims could not be generated for user: {user.Id}", ex);
                throw;
            }
        }

        private bool IsAuthenticationTokenValid(UserToken userToken)
        {
            return !(userToken.RefreshTokenExpiration <= DateTime.Now);
        }
    }
}