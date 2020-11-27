using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class ChatroomMessageRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly ChatroomMessageRepository chatroomMessageRepository;

        public ChatroomMessageRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
                .UseInMemoryDatabase(databaseName: "FlyIt-ChatroomMessage")
                .Options;

            flyItContext = new FlyItContext(options);

            chatroomMessageRepository = new ChatroomMessageRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.ChatroomMessage);
            flyItContext.RemoveRange(flyItContext.Chatroom);
            flyItContext.RemoveRange(flyItContext.Flight);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class AddChatroomMessageAsync : ChatroomMessageRepositoryTest
        {
            [TestMethod]
            public async Task CanAddChatroomMessage()
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

                var chatroomMessage = new ChatroomMessage()
                {
                    User = user,
                    UserId = user.Id,
                    Chatroom = chatroom,
                    ChatroomId = chatroom.Id,
                    Message = "Message",
                    DateTime = DateTime.Now,
                };

                var result = await chatroomMessageRepository.AddChatroomMessageAsync(chatroomMessage);
                var chatroomMessageInDatabase = await flyItContext.ChatroomMessage.SingleOrDefaultAsync(uc => uc.ChatroomId == chatroom.Id && uc.UserId == user.Id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(chatroomMessageInDatabase);
                Assert.AreEqual(chatroomMessage.Chatroom, result.Chatroom);
                Assert.AreEqual(chatroomMessage.User, result.User);
                Assert.AreEqual(chatroomMessage.Message, result.Message);
            }

            [TestMethod]
            public async Task ReturnsNullIfUserChatroomIsNull()
            {
                var result = await chatroomMessageRepository.AddChatroomMessageAsync(null);

                var userChatrooms = await flyItContext.ChatroomMessage.ToListAsync();

                Assert.IsNull(result);
                Assert.IsTrue(userChatrooms.Count < 1);
            }
        }
    }
}