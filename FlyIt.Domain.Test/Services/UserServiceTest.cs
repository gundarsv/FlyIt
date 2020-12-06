using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using FlyIt.Domain.Test.TestHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
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

        [TestClass]
        public class SignInUser : UserServiceTest
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { new UnexpectedResult<AuthenticationToken>() };
                    yield return new object[] { new SuccessResult<AuthenticationToken>(new AuthenticationToken()) };
                    yield return new object[] { new InvalidResult<AuthenticationToken>("invalid") };
                    yield return new object[] { new NotFoundResult<AuthenticationToken>("notFound") };
                }
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await userService.SignInUser(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfCheckPasswordFail()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

                var result = await userService.SignInUser(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [DataTestMethod]
            [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
            public async Task ReturnsTokenServiceResult(Result<AuthenticationToken> tokenServiceResult)
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
                tokenService.Setup(ts => ts.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>())).ReturnsAsync(tokenServiceResult);

                var result = await userService.SignInUser(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
                tokenService.Verify(ts => ts.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>()), Times.Once);

                Assert.AreEqual(tokenServiceResult.Data, result.Data);
                Assert.AreEqual(tokenServiceResult.ResultType, result.ResultType);
            }
        }

        [TestClass]
        public class SignInSystemAdministrator : UserServiceTest
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { new UnexpectedResult<AuthenticationToken>() };
                    yield return new object[] { new SuccessResult<AuthenticationToken>(new AuthenticationToken()) };
                    yield return new object[] { new InvalidResult<AuthenticationToken>("invalid") };
                    yield return new object[] { new NotFoundResult<AuthenticationToken>("notFound") };
                }
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await userService.SignInSystemAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotInRole()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
                {
                    Roles.AirportsAdministrator.ToString(),
                });

                var result = await userService.SignInSystemAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfCheckPasswordFail()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
                {
                    Roles.SystemAdministrator.ToString(),
                });

                userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

                var result = await userService.SignInSystemAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [DataTestMethod]
            [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
            public async Task ReturnsTokenServiceResult(Result<AuthenticationToken> tokenServiceResult)
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
                {
                    Roles.SystemAdministrator.ToString(),
                });
                userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
                tokenService.Setup(ts => ts.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>())).ReturnsAsync(tokenServiceResult);

                var result = await userService.SignInSystemAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
                tokenService.Verify(ts => ts.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>()), Times.Once);

                Assert.AreEqual(tokenServiceResult.Data, result.Data);
                Assert.AreEqual(tokenServiceResult.ResultType, result.ResultType);
            }
        }

        [TestClass]
        public class SignInAirportsAdministrator : UserServiceTest
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { new UnexpectedResult<AuthenticationToken>() };
                    yield return new object[] { new SuccessResult<AuthenticationToken>(new AuthenticationToken()) };
                    yield return new object[] { new InvalidResult<AuthenticationToken>("invalid") };
                    yield return new object[] { new NotFoundResult<AuthenticationToken>("notFound") };
                }
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await userService.SignInAirportsAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotInRole()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
                {
                    Roles.SystemAdministrator.ToString(),
                });

                var result = await userService.SignInAirportsAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfCheckPasswordFail()
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
                {
                    Roles.AirportsAdministrator.ToString(),
                });

                userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

                var result = await userService.SignInAirportsAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [DataTestMethod]
            [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
            public async Task ReturnsTokenServiceResult(Result<AuthenticationToken> tokenServiceResult)
            {
                userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
                {
                    Roles.AirportsAdministrator.ToString(),
                });
                userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
                tokenService.Setup(ts => ts.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>())).ReturnsAsync(tokenServiceResult);

                var result = await userService.SignInAirportsAdministrator(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
                tokenService.Verify(ts => ts.GenerateAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<AuthenticationFlow>()), Times.Once);

                Assert.AreEqual(tokenServiceResult.Data, result.Data);
                Assert.AreEqual(tokenServiceResult.ResultType, result.ResultType);
            }
        }

        [TestClass]
        public class GetUsers : UserServiceTest
        {
            [TestMethod]
            public async Task ReturnNotFoundIfEmpty()
            {
                var mockData = Enumerable.Empty<User>().AsQueryable();

                userManager.Setup(um => um.Users).Returns(new TestAsyncEnumerable<User>(mockData));

                var result = await userService.GetUsers();

                userManager.Verify(um => um.Users, Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccess()
            {
                var mockData = new List<User>() { new User(), new User() };

                userManager.Setup(um => um.Users).Returns(new TestAsyncEnumerable<User>(mockData.AsQueryable()));

                var result = await userService.GetUsers();

                userManager.Verify(um => um.Users, Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.Users).Throws(new Exception());

                var result = await userService.GetUsers();

                userManager.Verify(um => um.Users, Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class GetAirportsAdministrators : UserServiceTest
        {
            [TestMethod]
            public async Task ReturnNotFoundIfEmpty()
            {
                userManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(new List<User>());

                var result = await userService.GetAiportsAdministrators();

                userManager.Verify(um => um.GetUsersInRoleAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccess()
            {
                var mockData = new List<User>() { new User(), new User() };

                userManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(mockData);

                var result = await userService.GetAiportsAdministrators();

                userManager.Verify(um => um.GetUsersInRoleAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

                var result = await userService.GetAiportsAdministrators();

                userManager.Verify(um => um.GetUsersInRoleAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteUser : UserServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await userService.DeleteUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotDeleted()
            {
                var identityError = new IdentityError();
                identityError.Description = "Something went wrong";

                var identityerrorArray = new IdentityError[1] {
                    identityError
                };

                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(x => x.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed(identityError));

                var result = await userService.DeleteUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Once);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
                Assert.IsNull(result.Data);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfDeleted()
            {
                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(x => x.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

                var result = await userService.DeleteUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Once);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrows()
            {
                userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(x => x.DeleteAsync(It.IsAny<User>())).ThrowsAsync(new Exception());

                var result = await userService.DeleteUser(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Once);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}