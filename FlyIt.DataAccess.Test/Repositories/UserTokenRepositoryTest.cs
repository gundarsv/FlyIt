using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class UserTokenRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly UserTokenRepository tokenRepository;

        public UserTokenRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
                .UseInMemoryDatabase(databaseName: "FlyIt-UserToken")
                .Options;

            flyItContext = new FlyItContext(options);

            tokenRepository = new UserTokenRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.Users);
            flyItContext.RemoveRange(flyItContext.UserToken);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class UpdateUserTokenAsync : UserTokenRepositoryTest
        {
            [TestMethod]
            public async Task CanUpdateUserToken()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var userToken = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AuthenticationFlow = 1,
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                await flyItContext.AddRangeAsync(user, userToken);

                await flyItContext.SaveChangesAsync();

                var updatedToken = new UserToken()
                {
                    Id = userToken.Id,
                    User = user,
                    UserId = user.Id,
                    AccessToken = "AccessTokenNew",
                    RefreshToken = userToken.RefreshToken,
                    LoginProvider = userToken.LoginProvider,
                    AuthenticationFlow = userToken.AuthenticationFlow,
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = userToken.RefreshTokenExpiration,
                };

                var resultBeforeUpdating = await flyItContext.UserToken.SingleOrDefaultAsync(ut => ut.Id == userToken.Id);
                var accessTokenBeforeUpdate = resultBeforeUpdating.AccessToken;
                var result = await tokenRepository.UpdateUserTokenAsync(updatedToken);
                var resultAfterUpdating = await flyItContext.UserToken.SingleOrDefaultAsync(ut => ut.Id == userToken.Id);
                var accessTokenAfterUpdate = resultAfterUpdating.AccessToken;

                Assert.IsNotNull(result);
                Assert.AreEqual(resultBeforeUpdating.AccessToken, userToken.AccessToken);
                Assert.AreEqual(resultAfterUpdating.AccessToken, result.AccessToken);
                Assert.AreEqual(resultAfterUpdating.AccessToken, updatedToken.AccessToken);
                Assert.AreNotEqual(accessTokenAfterUpdate, accessTokenBeforeUpdate);
            }

            [TestMethod]
            public async Task ReturnsNullIfDoesNotExist()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var userToken = new UserToken()
                {
                    Id = 2,
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                var result = await tokenRepository.UpdateUserTokenAsync(userToken);

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ThrowsExceptionIfUserTokenIsNull()
            {
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
                {
                    await tokenRepository.UpdateUserTokenAsync(null);
                });
            }
        }

        [TestClass]
        public class AddUserTokenAsync : UserTokenRepositoryTest
        {
            [TestMethod]
            public async Task CanAddUserToken()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                await flyItContext.AddAsync(user);

                await flyItContext.SaveChangesAsync();

                var userToken = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                var result = await tokenRepository.AddUserTokenAsync(userToken);
                var userTokenInDatabase = await flyItContext.UserToken.SingleOrDefaultAsync(ut => ut.Id == userToken.Id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(userTokenInDatabase);
                Assert.AreEqual(userToken.Id, result.Id);
                Assert.AreEqual(userToken.RefreshToken, result.RefreshToken);
            }

            [TestMethod]
            public async Task ReturnsNullIfUserTokenIsNull()
            {
                var result = await tokenRepository.AddUserTokenAsync(null);

                var userTokens = await flyItContext.UserToken.ToListAsync();

                Assert.IsNull(result);
                Assert.IsTrue(userTokens.Count < 1);
            }
        }

        [TestClass]
        public class RemoveUserTokenAsync : UserTokenRepositoryTest
        {
            [TestMethod]
            public async Task CanRemoveUserFlight()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var userToken = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                await flyItContext.AddRangeAsync(user, userToken);

                await flyItContext.SaveChangesAsync();

                var result = await tokenRepository.RemoveUserTokenAsync(userToken);
                var afterRemove = await flyItContext.UserToken.ToListAsync();

                Assert.IsNotNull(result);
                Assert.IsTrue(afterRemove.Count < 1);
            }

            [TestMethod]
            public async Task ReturnsNullIfUserFlightIsNull()
            {
                var result = await tokenRepository.RemoveUserTokenAsync(null);

                var userTokens = await flyItContext.UserToken.ToListAsync();

                Assert.IsNull(result);
                Assert.IsTrue(userTokens.Count < 1);
            }
        }

        [TestClass]
        public class GetUserTokenByRefreshAndAccessTokenAsync : UserTokenRepositoryTest
        {
            [TestMethod]
            public async Task CanGetByRefreshAndAccessToken()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var userToken = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                var userToken2 = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken2",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                await flyItContext.AddRangeAsync(user, userToken, userToken2);

                await flyItContext.SaveChangesAsync();

                var result = await tokenRepository.GetUserTokenByRefreshAndAccessTokenAsync(userToken.RefreshToken, userToken.AccessToken);

                Assert.AreEqual(userToken.AccessToken, result.AccessToken);
                Assert.AreEqual(userToken.Id, result.Id);
                Assert.AreNotEqual(userToken2.AccessToken, result.AccessToken);
                Assert.AreNotEqual(userToken2.Id, result.Id);
            }

            [TestMethod]
            public async Task ReturnsNullIfValuesAreNull()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var userToken = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                var userToken2 = new UserToken()
                {
                    User = user,
                    UserId = user.Id,
                    AuthenticationFlow = 1,
                    AccessToken = "AccessToken2",
                    RefreshToken = "RefreshToken",
                    LoginProvider = "LoginProvider",
                    AccessTokenExpiration = DateTime.Now,
                    RefreshTokenExpiration = DateTime.Now,
                };

                await flyItContext.AddRangeAsync(user, userToken, userToken2);

                await flyItContext.SaveChangesAsync();

                var result = await tokenRepository.GetUserTokenByRefreshAndAccessTokenAsync(null, null);

                Assert.IsNull(result);
                Assert.IsTrue(flyItContext.UserToken.ToList().Count > 1);
            }
        }
    }
}