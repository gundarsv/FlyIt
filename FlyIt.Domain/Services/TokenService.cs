using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Settings;
using Microsoft.AspNetCore.Identity;
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

        public TokenService(IOptionsSnapshot<JWTSettings> tokenSettings, UserManager<Entity.User> userManager, IUserTokenRepository repository, IMapper mapper)
        {
            this.tokenSettings = tokenSettings.Value;
            this.repository = repository;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<Result<AuthenticationToken>> RefreshTokenAsync(string refreshToken, string accessToken)
        {
            try
            {
                if (!new JwtSecurityTokenHandler().CanReadToken(accessToken))
                {
                    return new InvalidResult<AuthenticationToken>("Invalid Token");
                }

                var token = DecodeAccessToken(accessToken);
                var userId = token.Claims.Where(claim => claim.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault().Value;
                var user = await userManager.FindByIdAsync(userId);
                var roles = await userManager.GetRolesAsync(user);
                var accessTokenExpiration = DateTime.Now.AddDays(Convert.ToDouble(tokenSettings.ExpirationInDays));

                var isTokenValid = repository.ValidateAuthenticationToken(user, refreshToken, accessToken);

                if (!isTokenValid)
                {
                    return new InvalidResult<AuthenticationToken>("Invalid Token");
                }

                var newAccessToken = GenerateAccessToken(user, accessTokenExpiration, tokenSettings.Secret, tokenSettings.Issuer, roles);

                var updatedToken = repository.UpdateUserToken(user, newAccessToken, accessTokenExpiration);

                var result = mapper.Map<UserToken, AuthenticationToken>(updatedToken);

                return new SuccessResult<AuthenticationToken>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AuthenticationToken>(ex.Message);
            }
            
        }

        public async Task<Result<AuthenticationToken>> GenerateAuthenticationTokenAsync(Entity.User user, string loginProvider)
        {
            try
            {
                var refreshTokenExpiration = DateTime.Now.AddDays(30);
                var accessTokenExpiration = DateTime.Now.AddDays(Convert.ToDouble(tokenSettings.ExpirationInDays));
                var roles = await userManager.GetRolesAsync(user);

                var accessToken = GenerateAccessToken(user, accessTokenExpiration, tokenSettings.Secret, tokenSettings.Issuer, roles);
                var refreshToken = GenerateRefreshToken();

                repository.RemoveUserToken(user, loginProvider);

                var token = repository.AddUserToken(user, accessToken, refreshToken, accessTokenExpiration, refreshTokenExpiration, loginProvider);

                var result = mapper.Map<UserToken, AuthenticationToken>(token);

                return new SuccessResult<AuthenticationToken>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<AuthenticationToken>(ex.Message);
            }
        }

        private string GenerateAccessToken(Entity.User user, DateTime expiresAt, string secret, string issuer, IList<string> roles)
        {
            var claims = GetUserClaims(user, roles);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken DecodeAccessToken(string accessToken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private IList<Claim> GetUserClaims(Entity.User user, IList<string> roles)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);

            return claims;
        }
    }
}
