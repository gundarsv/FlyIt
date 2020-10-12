using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class UserServiceTest
    {
        private readonly Mock<UserManager<User>> userManager;
        private readonly Mock<ITokenService> tokenService;
        private readonly IMapper mapper;
        private readonly UserService userService;

        public UserServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);
            this.tokenService = new Mock<ITokenService>();

            var userMappingProfile = new UserMapping();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(userMappingProfile));
            mapper = new Mapper(configuration);

            userService = new UserService(userManager.Object, tokenService.Object, mapper);
        }

        [TestClass]
        public class GetUser : UserServiceTest
        {
            [TestMethod]
            public async Task ReturnsUserAsync()
            {
                var user = new User
                {
                    Id = 1
                };

                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

                var result = await userService.GetUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data);
            }

            [TestMethod]
            public async Task ReturnsNotFoundAsync()
            {
                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await userService.GetUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Throws(new Exception());

                var result = await userService.GetUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.AreEqual(result.ResultType, ResultType.Unexpected);
                Assert.IsNull(result.Data);
            }
        }

        [TestClass]
        public class CreateUser : UserServiceTest
        {
            [TestMethod]
            public async Task CreatesUserAsync()
            {
                userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

                var result = await userService.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
                Assert.AreEqual(ResultType.Created, result.ResultType);
            }

            [TestMethod]
            public async Task DoesNotCreateUserAsync()
            {
                var identityError = new IdentityError();
                identityError.Description = "Something went wrong";

                var identityerrorArray = new IdentityError[1] {
                    identityError
                };

                userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(identityerrorArray));

                var result = await userService.CreateUser("Test", "Test", "Test");

                userManager.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
                Assert.IsNotNull(result.Errors);
                Assert.IsNull(result.Data);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Throws(new Exception());

                var result = await userService.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
