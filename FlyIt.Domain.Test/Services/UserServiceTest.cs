using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test
{
    [TestClass]
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
            public void ReturnsUser()
            {
                var user = new User
                {
                    Id = 1
                };

                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(user));

                var result = userService.GetUser(It.IsAny<ClaimsPrincipal>()).Result;

                Assert.AreEqual(result.ResultType, ResultType.Ok);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(result.Data.Id, user.Id);
            }

            [TestMethod]
            public void ReturnsNotFound()
            {
                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult<User>(null));

                var result = userService.GetUser(It.IsAny<ClaimsPrincipal>()).Result;

                Assert.AreEqual(result.ResultType, ResultType.NotFound);
                Assert.IsNull(result.Data);
            }
        }
        
    }
}
