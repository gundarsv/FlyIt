using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class ChatroomRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly ChatroomRepository chatroomRepsitory;

        public ChatroomRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
                .UseInMemoryDatabase(databaseName: "FlyIt-Chatroom")
                .Options;

            flyItContext = new FlyItContext(options);

            chatroomRepsitory = new ChatroomRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.Chatroom);
            flyItContext.RemoveRange(flyItContext.Flight);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class GetChatroomByIdAsync : ChatroomRepositoryTest
        {
            [TestMethod]
            public async Task CanGetChatroomById()
            {
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

                var result = await chatroomRepsitory.GetChatroomByIdAsync(chatroom.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(chatroom.Flight, result.Flight);
                Assert.AreEqual(chatroom.Id, result.Id);
            }

            [TestMethod]
            public async Task ReturnsNullIfNotFound()
            {
                var result = await chatroomRepsitory.GetChatroomByIdAsync(0);

                Assert.IsNull(result);
            }
        }
    }
}