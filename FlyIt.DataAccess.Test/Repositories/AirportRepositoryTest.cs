﻿using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class AirportRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly AirportRepository airportRepository;

        public AirportRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
            .UseInMemoryDatabase(databaseName: "FlyIt")
            .Options;

            flyItContext = new FlyItContext(options);

            airportRepository = new AirportRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.Users);
            flyItContext.RemoveRange(flyItContext.Airport);
            flyItContext.RemoveRange(flyItContext.UserAirport);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class GetAirportsAsync : AirportRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnAirports()
            {
                List<Airport> airports = new List<Airport>()
                {
                    new Airport()
                    {
                        Iata = "RIX",
                        Name = "Riga"
                    },
                    new Airport()
                    {
                        Iata = "BLL",
                        Name = "BILLUND"
                    }
                };

                await flyItContext.Airport.AddRangeAsync(airports);

                await flyItContext.SaveChangesAsync(default);

                var result = await airportRepository.GetAirportsAsync();

                Assert.IsNotNull(result);
                Assert.AreEqual(airports.Count, result.Count);
            }
        }

        [TestClass]
        public class GetAirportByIdAsync : AirportRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnAirportById()
            {
                var airport = new Airport()
                {
                    Id = 3,
                    Iata = "NYK",
                    Name = "New York"
                };

                List<Airport> airports = new List<Airport>()
                {
                    new Airport()
                    {
                        Id = 1,
                        Iata = "RIX",
                        Name = "Riga"
                    },
                    new Airport()
                    {
                        Id = 2,
                        Iata = "BLL",
                        Name = "BILLUND"
                    },
                    airport,
                };

                await flyItContext.Airport.AddRangeAsync(airports);

                await flyItContext.SaveChangesAsync(default);

                var result = await airportRepository.GetAirportByIdAsync(airport.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(airport.Id, result.Id);
                Assert.AreEqual(airport.Name, result.Name);
                Assert.AreEqual(airport.Iata, result.Iata);
            }
        }

        [TestClass]
        public class AddUserAirportAsync : AirportRepositoryTest
        {
            [TestMethod]
            public async Task CanAddAirportToUser()
            {
                var airport = new Airport()
                {
                    Id = 2,
                    Iata = "BLL",
                    Name = "BILLUND"
                };

                List<Airport> airports = new List<Airport>()
                {
                    new Airport()
                    {
                        Id = 1,
                        Iata = "RIX",
                        Name = "Riga"
                    },
                    airport,
                };

                var user = new User()
                {
                    Id = 1,
                    Email = "email@email.com"
                };

                List<User> users = new List<User>()
                {
                    user,
                    new User()
                    {
                        Id = 4,
                        Email = "notemail@email.com",
                    }
                };

                await flyItContext.Users.AddRangeAsync(users);

                await flyItContext.Airport.AddRangeAsync(airports);

                await flyItContext.SaveChangesAsync();

                var result = await airportRepository.AddUserAirportAsync(user, airport);

                Assert.IsNotNull(result);
                Assert.AreEqual(airport.Id, result.AirportId);
                Assert.AreEqual(user.Id, result.UserId);
            }
        }

        [TestClass]
        public class GetUserAirportByIdAsync : AirportRepositoryTest
        {
            [TestMethod]
            public async Task CanGetUserAirportById()
            {
                var user = new User()
                {
                    Id = 1
                };

                var airport = new Airport()
                {
                    Id = 1
                };

                var userAirport = new UserAirport()
                {
                    Airport = airport,
                    AirportId = airport.Id,
                    UserId = user.Id,
                    User = user
                };

                await flyItContext.AddRangeAsync(user, airport, userAirport);

                await flyItContext.SaveChangesAsync();

                var result = await airportRepository.GetUserAirportByIdAsync(user.Id, airport.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(userAirport, result);
            }

            [TestMethod]
            public async Task ReturnsNullIfNotFound()
            {
                var user = new User()
                {
                    Id = 1
                };

                var airport = new Airport()
                {
                    Id = 1
                };

                await flyItContext.AddRangeAsync(user, airport);

                await flyItContext.SaveChangesAsync();

                var result = await airportRepository.GetUserAirportByIdAsync(user.Id, airport.Id);

                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class RemoveUserAirportAsync : AirportRepositoryTest
        {
            [TestMethod]
            public async Task CanRemoveUserAirport()
            {
                var user = new User()
                {
                    Id = 1
                };

                var airport = new Airport()
                {
                    Id = 1
                };

                var userAirport = new UserAirport()
                {
                    Airport = airport,
                    AirportId = airport.Id,
                    UserId = user.Id,
                    User = user
                };

                await flyItContext.AddRangeAsync(user, airport, userAirport);

                await flyItContext.SaveChangesAsync();

                var result = await airportRepository.RemoveUserAirportAsync(userAirport);
                var resultInDatabase = await flyItContext.UserAirport.SingleOrDefaultAsync(ua => ua.AirportId == airport.Id && ua.UserId == user.Id);

                Assert.IsNotNull(result);
                Assert.IsNull(resultInDatabase);
            }
        }
    }
}