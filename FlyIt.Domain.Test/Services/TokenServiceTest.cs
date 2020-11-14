using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using FlyIt.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class TokenServiceTest
    {
        private readonly Mock<IOptionsSnapshot<JWTSettings>> tokenSettings;
        private readonly Mock<IUserTokenRepository> repository;
        private readonly Mock<UserManager<User>> userManager;
        private readonly Mock<ILogger<TokenService>> logger;
        private readonly IMapper mapper;
        private readonly TokenService service;

        public TokenServiceTest()
        {
            tokenSettings = new Mock<IOptionsSnapshot<JWTSettings>>();
            tokenSettings.Setup(ts => ts.Value).Returns(new JWTSettings()
            {
                Issuer = "Issuer",
                RefreshTokenExpirationInDays = 1,
                AccessTokenExpirationInDays = 1,
                Secret = "YAw8IqNHJNsckk17XvNhX4Hr2wOUsgO8KLQkxWPqPaPf75buDiArjWSd2GS622g6aKbfe_mPaRAcfohiQAWD46n5pPR5PLzh6yhUImThLpPfj6MIpGXp23476JhVRA5Hpj8KdBQoiik7xFlWIvUtW0SkJ7FiMLYRedE7foOMWOUDoy7qNCa7zSAt0OOhFtS2fYFtezd3_bdW1rKh8R1xci_R8ASpr4f9sevu28oUwrr7QK7K3H95IF8onkkXuIyoD9V4zKpddddDcljLeuYr3GhUkhJtMPvatvrxKWQVsS42Kdgj696UzrghYwz-7qD3MwSsZfYIiN649SRoBfzG6Q"
            });

            repository = new Mock<IUserTokenRepository>();
            logger = new Mock<ILogger<TokenService>>();

            var tokenMappingProfile = new AuthenticaiontMappingProfile();

            var userStore = Mock.Of<IUserStore<User>>();
            userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(tokenMappingProfile));
            mapper = new Mapper(configuration);

            service = new TokenService(tokenSettings.Object, userManager.Object, repository.Object, mapper, logger.Object);
        }

        [TestClass]
        public class RefreshTokenAsync : TokenServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfTokenNotFound()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserToken)null);

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken());
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotValidAndCantRemove()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken()
                {
                    RefreshTokenExpiration = DateTime.Now.AddDays(-2)
                });
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.RemoveUserTokenAsync(It.IsAny<UserToken>())).ReturnsAsync((UserToken)null);

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.RemoveUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotValidAndRemoved()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken()
                {
                    RefreshTokenExpiration = DateTime.Now.AddDays(-2)
                });
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.RemoveUserTokenAsync(It.IsAny<UserToken>())).ReturnsAsync(new UserToken());

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.RemoveUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfValidAndCantGenerate()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken()
                {
                    RefreshTokenExpiration = DateTime.Now.AddDays(1)
                });
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfValidAndCantUpdate()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken()
                {
                    RefreshTokenExpiration = DateTime.Now.AddDays(1)
                });
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User()
                {
                    UserName = "UserName"
                });
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                repository.Setup(r => r.UpdateUserTokenAsync(It.IsAny<UserToken>())).ReturnsAsync((UserToken)null);

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.UpdateUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfValidAndCanUpdate()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken()
                {
                    RefreshTokenExpiration = DateTime.Now.AddDays(1)
                });
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User()
                {
                    UserName = "UserName"
                });
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                repository.Setup(r => r.UpdateUserTokenAsync(It.IsAny<UserToken>())).ReturnsAsync(new UserToken()
                {
                    AccessTokenExpiration = DateTime.Now
                });

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.UpdateUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                repository.Setup(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserToken()
                {
                    RefreshTokenExpiration = DateTime.Now.AddDays(1)
                });
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User()
                {
                    UserName = "UserName"
                });
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                repository.Setup(r => r.UpdateUserTokenAsync(It.IsAny<UserToken>())).ThrowsAsync(new Exception());

                var result = await service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>());

                repository.Verify(r => r.GetUserTokenByRefreshAndAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.UpdateUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class GenerateAuthenticationTokenAsync : TokenServiceTest
        {
            [TestMethod]
            public async Task RetunsInvalidIfCantGenerate()
            {
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await service.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>());

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task RetunsInvalidIfCantAdd()
            {
                var user = new User()
                {
                    UserName = "userName"
                };

                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                repository.Setup(r => r.AddUserTokenAsync(It.IsAny<UserToken>())).ReturnsAsync((UserToken)null);

                var result = await service.GenerateAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<AuthenticationFlow>());

                repository.Verify(r => r.AddUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task RetunsSuccessIfAdded()
            {
                var user = new User()
                {
                    UserName = "userName"
                };

                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                repository.Setup(r => r.AddUserTokenAsync(It.IsAny<UserToken>())).ReturnsAsync(new UserToken()
                {
                    AccessTokenExpiration = DateTime.Now
                });

                var result = await service.GenerateAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<AuthenticationFlow>());

                repository.Verify(r => r.AddUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrows()
            {
                var user = new User()
                {
                    UserName = "userName"
                };

                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                repository.Setup(r => r.AddUserTokenAsync(It.IsAny<UserToken>())).ThrowsAsync(new Exception());

                var result = await service.GenerateAuthenticationTokenAsync(user, It.IsAny<string>(), It.IsAny<AuthenticationFlow>());

                repository.Verify(r => r.AddUserTokenAsync(It.IsAny<UserToken>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }
    }
}