using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class UserChatroomRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly UserChatroomRepository userChatroomRepository;

        public UserChatroomRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
                .UseInMemoryDatabase(databaseName: "FlyIt-UserChatroom")
                .Options;

            flyItContext = new FlyItContext(options);

            userChatroomRepository = new UserChatroomRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.UserChatroom);
            flyItContext.RemoveRange(flyItContext.Users);
            flyItContext.RemoveRange(flyItContext.Chatroom);
            flyItContext.RemoveRange(flyItContext.Flight);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class AddUserChatroomAsync : UserChatroomRepositoryTest
        {
            [TestMethod]
            public async Task CanAddUserChatroom()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                await flyItContext.AddAsync(user);

                var flight = new Flight()
                {
                    FlightNo = "FlightNo"
                };

                await flyItContext.AddAsync(flight);

                var chatroom = new Chatroom()
                {
                    Flight = flight,
                    FlightId = flight.Id,
                };

                await flyItContext.AddAsync(chatroom);

                await flyItContext.SaveChangesAsync();

                var userChatroom = new UserChatroom()
                {
                    User = user,
                    UserId = user.Id,
                    Chatroom = chatroom,
                    ChatroomId = chatroom.Id
                };

                var result = await userChatroomRepository.AddUserChatroomAsync(userChatroom);

                var userChatroomInDatabase = await flyItContext.UserChatroom.SingleOrDefaultAsync(uc => uc.ChatroomId == chatroom.Id && uc.UserId == user.Id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(userChatroomInDatabase);
                Assert.AreEqual(userChatroom.Chatroom, result.Chatroom);
                Assert.AreEqual(userChatroom.User, result.User);
            }

            [TestMethod]
            public async Task ReturnsNullIfUserChatroomIsNull()
            {
                var result = await userChatroomRepository.AddUserChatroomAsync(null);

                var userChatrooms = await flyItContext.UserChatroom.ToListAsync();

                Assert.IsNull(result);
                Assert.IsTrue(userChatrooms.Count < 1);
            }
        }

        [TestClass]
        public class RemoveUserChatroomAsync : UserChatroomRepositoryTest
        {
            [TestMethod]
            public async Task CanRemoveUserChatroom()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                await flyItContext.AddAsync(user);

                var flight = new Flight()
                {
                    FlightNo = "FlightNo"
                };

                await flyItContext.AddAsync(flight);

                var chatroom = new Chatroom()
                {
                    Flight = flight,
                    FlightId = flight.Id,
                };

                await flyItContext.AddAsync(chatroom);

                var userChatroom = new UserChatroom()
                {
                    Chatroom = chatroom,
                    User = user,
                    ChatroomId = chatroom.Id,
                    UserId = user.Id
                };

                await flyItContext.AddAsync(userChatroom);

                await flyItContext.SaveChangesAsync();

                var result = await userChatroomRepository.RemoveUserChatroomAsync(userChatroom);
                var afterRemove = await flyItContext.UserChatroom.ToListAsync();

                Assert.IsNotNull(result);
                Assert.IsTrue(afterRemove.Count < 1);
            }

            [TestMethod]
            public async Task ReturnsNullIfUserFlightIsNull()
            {
                var result = await userChatroomRepository.RemoveUserChatroomAsync(null);

                var userTokens = await flyItContext.UserChatroom.ToListAsync();

                Assert.IsNull(result);
                Assert.IsTrue(userTokens.Count < 1);
            }
        }

        [TestClass]
        public class GetUserChatroomByIdAsync : UserChatroomRepositoryTest
        {
            [TestMethod]
            public async Task CanGetUserChatroomById()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                await flyItContext.AddAsync(user);

                var flight = new Flight()
                {
                    FlightNo = "FlightNo"
                };

                await flyItContext.AddAsync(flight);

                var chatroom = new Chatroom()
                {
                    Flight = flight,
                    FlightId = flight.Id,
                };

                await flyItContext.AddAsync(chatroom);

                var userChatroom = new UserChatroom()
                {
                    Chatroom = chatroom,
                    User = user,
                    ChatroomId = chatroom.Id,
                    UserId = user.Id
                };

                await flyItContext.AddAsync(userChatroom);

                await flyItContext.SaveChangesAsync();

                var result = await userChatroomRepository.GetUserChatroomByIdAsync(chatroom.Id, user.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(userChatroom.Chatroom, result.Chatroom);
                Assert.AreEqual(userChatroom.User, result.User);
            }

            [TestMethod]
            public async Task ReturnsNullIfNotFound()
            {
                var result = await userChatroomRepository.GetUserChatroomByIdAsync(0, 0);

                Assert.IsNull(result);
            }
        }
    }
}