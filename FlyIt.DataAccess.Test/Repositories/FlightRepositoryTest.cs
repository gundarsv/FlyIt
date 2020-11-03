using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class FlightRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly FlightRepository repository;

        public FlightRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
                .UseInMemoryDatabase(databaseName: "FlyIt-Flights")
                .Options;

            flyItContext = new FlyItContext(options);

            repository = new FlightRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.Flight);
            flyItContext.RemoveRange(flyItContext.UserFlight);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class AddFlightAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanAddFlight()
            {
                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                var result = await repository.AddFlightAsync(flight);
                var flightInDatabase = await flyItContext.Flight.SingleOrDefaultAsync(f => f.FlightNo == flight.FlightNo);

                Assert.IsNotNull(result);
                Assert.IsNotNull(flightInDatabase);
                Assert.AreEqual(flight.Id, result.Id);
                Assert.AreEqual(flight.FlightNo, result.FlightNo);
            }

            [TestMethod]
            public async Task ThrowsExceptionIfNull()
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                {
                    await repository.AddFlightAsync(null);
                });

                var flights = await flyItContext.Flight.ToListAsync();

                Assert.IsTrue(flights.Count < 1);
            }
        }

        [TestClass]
        public class GetFlightByIdAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnById()
            {
                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                await flyItContext.AddAsync(flight);
                await flyItContext.SaveChangesAsync();

                var result = await repository.GetFlightByIdAsync(flight.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(flight.FlightNo, result.FlightNo);
                Assert.AreEqual(flight.Id, result.Id);
            }

            [TestMethod]
            public async Task ReturnsNullIfNotFound()
            {
                var result = await repository.GetFlightByIdAsync(670);

                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class AddUserFlightAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CaddAddUserFlight()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                await flyItContext.AddRangeAsync(user, flight);

                await flyItContext.SaveChangesAsync();

                var result = await repository.AddUserFlightAsync(user, flight);

                Assert.IsNotNull(result);
                Assert.AreEqual(flight, result.Flight);
                Assert.AreEqual(user, result.User);
            }

            [TestMethod]
            public async Task ReturnsNullIfUserOrFlightIsNull()
            {
                var result = await repository.AddUserFlightAsync(null, null);

                var userFlights = await flyItContext.UserFlight.ToListAsync();

                Assert.IsNull(result);
                Assert.IsTrue(userFlights.Count < 1);
            }
        }

        [TestClass]
        public class GetUserFlightsAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnUserFlights()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                var flight2 = new Flight()
                {
                    FlightNo = "BT148"
                };

                var userFlights = new List<UserFlight>()
                {
                   new UserFlight()
                    {
                        Flight = flight,
                        User = user,
                    },
                    new UserFlight()
                    {
                        Flight = flight2,
                        User = user,
                    },
                };

                await flyItContext.AddRangeAsync(user, flight, flight2);
                await flyItContext.UserFlight.AddRangeAsync(userFlights);

                await flyItContext.SaveChangesAsync();

                var result = await repository.GetUserFlightsAsync(user);

                Assert.AreEqual(userFlights.Count, result.Count);
                Assert.IsNotNull(result.FirstOrDefault().Flight);
            }

            [TestMethod]
            public async Task ReturnsEmptyIfDoesNotExist()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var user2 = new User()
                {
                    Email = "test@email.com"
                };

                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                var flight2 = new Flight()
                {
                    FlightNo = "BT148"
                };

                var userFlights = new List<UserFlight>()
                {
                   new UserFlight()
                    {
                        Flight = flight,
                        User = user,
                    },
                    new UserFlight()
                    {
                        Flight = flight2,
                        User = user,
                    },
                };

                await flyItContext.AddRangeAsync(user, user2, flight, flight2);
                await flyItContext.UserFlight.AddRangeAsync(userFlights);

                await flyItContext.SaveChangesAsync();

                var result = await repository.GetUserFlightsAsync(user2);

                Assert.IsTrue(result.Count < 1);
                Assert.IsNull(result.FirstOrDefault());
            }

            [TestMethod]
            public async Task ThrowsExceptionIfNull()
            {
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
                {
                    await repository.GetUserFlightsAsync(null);
                });

                var flights = await flyItContext.UserFlight.ToListAsync();

                Assert.IsTrue(flights.Count < 1);
            }
        }

        [TestClass]
        public class GetFlightByDateAndFlightNumber : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanGetByDateAndFlightNumber()
            {
                var flight = new Flight()
                {
                    Date = new DateTimeOffset(DateTime.Now),
                    FlightNo = "BT147"
                };

                var flight2 = new Flight()
                {
                    Date = new DateTimeOffset(DateTime.Now),
                    FlightNo = "BT149"
                };

                await flyItContext.AddRangeAsync(flight, flight2);
                await flyItContext.SaveChangesAsync();

                var result = await repository.GetFlightByDateAndFlightNumberAsync(flight.Date, flight.FlightNo);

                Assert.IsNotNull(result);
                Assert.AreEqual(flight, result);
            }

            [TestMethod]
            public async Task ReturnsNullIfNotFound()
            {
                var result = await repository.GetFlightByDateAndFlightNumberAsync(new DateTimeOffset(DateTime.Now), "BT147");

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsNullIfFlightNoIsNull()
            {
                var result = await repository.GetFlightByDateAndFlightNumberAsync(new DateTimeOffset(DateTime.Now), null);

                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class RemoveUserFlightAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanRemoveUserFlight()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                var flight2 = new Flight()
                {
                    FlightNo = "BT148"
                };

                var userFlight = new UserFlight()
                {
                    Flight = flight,
                    User = user,
                };

                var userFlights = new List<UserFlight>()
                {
                    userFlight,
                    new UserFlight()
                    {
                        Flight = flight2,
                        User = user,
                    },
                };

                await flyItContext.AddRangeAsync(user, flight, flight2);
                await flyItContext.UserFlight.AddRangeAsync(userFlights);

                await flyItContext.SaveChangesAsync();

                var databaseBefore = await flyItContext.UserFlight.Where(uf => uf.UserId == user.Id).ToListAsync();
                var result = await repository.RemoveUserFlightAsync(userFlight);
                var databaseAfter = await flyItContext.UserFlight.Where(uf => uf.UserId == user.Id).ToListAsync();

                Assert.IsTrue(databaseBefore.Count == userFlights.Count);
                Assert.IsTrue(databaseAfter.Count != userFlights.Count);
                Assert.IsNotNull(result);
            }

            [TestMethod]
            public async Task ThrowsExceptionIfUserFlightIsNull()
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                {
                    await repository.RemoveUserFlightAsync(null);
                });
            }
        }

        [TestClass]
        public class GetUserFlightByIdAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnUserFlightById()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                var flight2 = new Flight()
                {
                    FlightNo = "BT148"
                };

                var userFlight = new UserFlight()
                {
                    Flight = flight,
                    User = user,
                };

                var userFlights = new List<UserFlight>()
                {
                    userFlight,
                    new UserFlight()
                    {
                        Flight = flight2,
                        User = user,
                    },
                };

                await flyItContext.AddRangeAsync(user, flight, flight2);
                await flyItContext.UserFlight.AddRangeAsync(userFlights);

                await flyItContext.SaveChangesAsync();

                var result = await repository.GetUserFlightByIdAsync(user.Id, flight.Id);

                Assert.AreEqual(userFlight, result);
                Assert.IsNotNull(result.Flight);
            }

            [TestMethod]
            public async Task ReturnsEmptyIfDoesNotExist()
            {
                var user = new User()
                {
                    Email = "email@email.com"
                };

                var user2 = new User()
                {
                    Email = "test@email.com"
                };

                var flight = new Flight()
                {
                    FlightNo = "BT147"
                };

                var flight2 = new Flight()
                {
                    FlightNo = "BT148"
                };

                var userFlight = new UserFlight()
                {
                    Flight = flight,
                    User = user,
                };

                var userFlights = new List<UserFlight>()
                {
                    userFlight,
                    new UserFlight()
                    {
                        Flight = flight2,
                        User = user,
                    },
                };

                await flyItContext.AddRangeAsync(user, user2, flight, flight2);
                await flyItContext.UserFlight.AddRangeAsync(userFlights);

                await flyItContext.SaveChangesAsync();

                var result = await repository.GetUserFlightByIdAsync(user2.Id, flight.Id);

                Assert.IsTrue(result != userFlight);
                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class UpdateFlightAsync : FlightRepositoryTest
        {
            [TestMethod]
            public async Task CanUpdateFlight()
            {
                var flight = new Flight()
                {
                    FlightNo = "BT147",
                };

                await flyItContext.Flight.AddAsync(flight);

                await flyItContext.SaveChangesAsync();

                var updatedFlight = new Flight()
                {
                    Id = flight.Id,
                    FlightNo = "BT148"
                };

                var resultBeforeUpdating = await flyItContext.Flight.SingleOrDefaultAsync(f => f.Id == flight.Id);
                var result = await repository.UpdateFlightAsync(updatedFlight);
                var resultAfterUpdating = await flyItContext.Flight.SingleOrDefaultAsync(f => f.Id == flight.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(resultBeforeUpdating.FlightNo, flight.FlightNo);
                Assert.AreEqual(resultAfterUpdating.FlightNo, result.FlightNo);
                Assert.AreEqual(resultAfterUpdating.FlightNo, updatedFlight.FlightNo);
            }

            [TestMethod]
            public async Task ReturnsNullIfDoesNotExist()
            {
                var flight = new Flight()
                {
                    FlightNo = "BT147",
                };

                var result = await repository.UpdateFlightAsync(flight);

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ThrowsExceptionIfFlightIsNull()
            {
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
                {
                    await repository.UpdateFlightAsync(null);
                });
            }
        }
    }
}