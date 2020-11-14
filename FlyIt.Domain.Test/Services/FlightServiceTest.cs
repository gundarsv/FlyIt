using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.AviationstackResponses;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Entity = FlyIt.DataAccess.Entities;

namespace FlyIt.Domain.Test.Services
{
    public class FlightServiceTest
    {
        private readonly Mock<IAviationstackFlightService> aviationstackFlightService;
        private readonly IMapper mapper;
        private readonly Mock<IFlightRepository> repository;
        private readonly Mock<UserManager<User>> userManager;
        private readonly ICompareLogic compareLogic;
        private readonly FlightService service;

        public FlightServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            this.repository = new Mock<IFlightRepository>();
            var fliightMappingProfile = new FlightMapping();

            this.aviationstackFlightService = new Mock<IAviationstackFlightService>();

            this.compareLogic = new CompareLogic();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(fliightMappingProfile));
            this.mapper = new Mapper(configuration);

            this.service = new FlightService(aviationstackFlightService.Object, mapper, repository.Object, userManager.Object, compareLogic);
        }

        [TestClass]
        public class AddFlight : FlightServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<FlightSearchDTO>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfFlightNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync((FlightsResponse)null);

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfFlightNotInDatabaseButNotAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync((Entity.Flight)null);
                repository.Setup(r => r.AddFlightAsync(It.IsAny<Entity.Flight>())).ReturnsAsync((Entity.Flight)null);

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.AddFlightAsync(It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfFlightNotInDatabaseButNotAddedToUser()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync((Entity.Flight)null);
                repository.Setup(r => r.AddFlightAsync(It.IsAny<Entity.Flight>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>())).ReturnsAsync((Entity.UserFlight)null);

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.AddFlightAsync(It.IsAny<Entity.Flight>()), Times.Once);
                repository.Verify(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfFlightNotInDatabaseButAddedToUser()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync((Entity.Flight)null);
                repository.Setup(r => r.AddFlightAsync(It.IsAny<Entity.Flight>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>())).ReturnsAsync(new Entity.UserFlight());

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.AddFlightAsync(It.IsAny<Entity.Flight>()), Times.Once);
                repository.Verify(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvaldIfFlightInDatabaseButAlreadyAssigned()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight());

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvaldIfFlightInDatabaseButCannotBeAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Entity.UserFlight)null);
                repository.Setup(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>())).ReturnsAsync((Entity.UserFlight)null);

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfFlightInDatabaseAndAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Entity.UserFlight)null);
                repository.Setup(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>())).ReturnsAsync(new Entity.UserFlight());

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnUnexpectedIIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    },
                });

                repository.Setup(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Entity.UserFlight)null);
                repository.Setup(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>())).ThrowsAsync(new Exception());

                var result = await service.AddFlight(It.IsAny<ClaimsPrincipal>(), new FlightSearchDTO());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetFlightByDateAndFlightNumberAsync(It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserFlightAsync(It.IsAny<User>(), It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteFlight : FlightServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await service.DeleteFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfFlightNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync((Entity.Flight)null);

                var result = await service.DeleteFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnInvalidIfUserNotAssigned()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Entity.UserFlight)null);

                var result = await service.DeleteFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnInvalidIfNotRemoved()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight());
                repository.Setup(r => r.RemoveUserFlightAsync(It.IsAny<Entity.UserFlight>())).ReturnsAsync((Entity.UserFlight)null);

                var result = await service.DeleteFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.RemoveUserFlightAsync(It.IsAny<Entity.UserFlight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfRemoved()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight());
                repository.Setup(r => r.RemoveUserFlightAsync(It.IsAny<Entity.UserFlight>())).ReturnsAsync(new Entity.UserFlight());

                var result = await service.DeleteFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.RemoveUserFlightAsync(It.IsAny<Entity.UserFlight>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight());
                repository.Setup(r => r.RemoveUserFlightAsync(It.IsAny<Entity.UserFlight>())).ThrowsAsync(new Exception());

                var result = await service.DeleteFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.RemoveUserFlightAsync(It.IsAny<Entity.UserFlight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class GetFlight : FlightServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfFlightNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync((Entity.Flight)null);

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnInvalidIfUserNotAssigned()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Entity.UserFlight)null);

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfCanNotBeUpdated()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        Date = DateTimeOffset.Now.AddDays(-8)
                    }
                });

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfCanBeUpdatedButNoData()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        FlightNo = "FLIGHTNO",
                        Date = DateTimeOffset.Now
                    }
                });
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync((FlightsResponse)null);

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfCanBeUpdatedButDatesAreNotEqual()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        FlightNo = "FLIGHTNO",
                        Date = DateTimeOffset.Now
                    }
                });
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now.AddDays(-1),
                           Flight = new Flight()
                           {
                               Iata = "IATA"
                           }
                       }
                    }
                });

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfCanBeUpdatedDatesAreEqualAndCompareAreEqual()
            {
                var userFlight = new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        FlightNo = "FLIGHTNO",
                        Date = DateTimeOffset.Now,
                        Status = "Landed"
                    }
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userFlight);
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = userFlight.Flight.Date,
                           Flight = new Flight()
                           {
                               Iata = userFlight.Flight.FlightNo,
                           },
                           FlightStatus = userFlight.Flight.Status,
                       }
                    }
                });

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnInvalidIfCanBeUpdatedDatesAreEqualCompareAreNotEqualCanNotBeUpdated()
            {
                var userFlight = new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        FlightNo = "FLIGHTNO",
                        Date = DateTimeOffset.Now,
                        Status = "Landed"
                    }
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userFlight);
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = userFlight.Flight.Date,
                           Flight = new Flight()
                           {
                               Iata = userFlight.Flight.FlightNo,
                           },
                           FlightStatus = "Active",
                       }
                    }
                });
                repository.Setup(r => r.UpdateFlightAsync(It.IsAny<Entity.Flight>())).ReturnsAsync((Entity.Flight)null);

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.UpdateFlightAsync(It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnSuccessIfCanBeUpdatedDatesAreEqualCompareAreNotEqualCanBeUpdated()
            {
                var userFlight = new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        FlightNo = "FLIGHTNO",
                        Date = DateTimeOffset.Now,
                        Status = "Landed"
                    }
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userFlight);
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = userFlight.Flight.Date,
                           Flight = new Flight()
                           {
                               Iata = userFlight.Flight.FlightNo,
                           },
                           FlightStatus = "Active",
                       }
                    }
                });
                repository.Setup(r => r.UpdateFlightAsync(It.IsAny<Entity.Flight>())).ReturnsAsync(new Entity.Flight());

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.UpdateFlightAsync(It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnUnexpectedIfThrowsException()
            {
                var userFlight = new Entity.UserFlight()
                {
                    Flight = new Entity.Flight()
                    {
                        FlightNo = "FLIGHTNO",
                        Date = DateTimeOffset.Now,
                        Status = "Landed"
                    }
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetFlightByIdAsync(It.IsAny<int>())).ReturnsAsync(new Entity.Flight());
                repository.Setup(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userFlight);
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = userFlight.Flight.Date,
                           Flight = new Flight()
                           {
                               Iata = userFlight.Flight.FlightNo,
                           },
                           FlightStatus = "Active",
                       }
                    }
                });
                repository.Setup(r => r.UpdateFlightAsync(It.IsAny<Entity.Flight>())).ThrowsAsync(new Exception());

                var result = await service.GetFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetFlightByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserFlightByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.UpdateFlightAsync(It.IsAny<Entity.Flight>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class GetUserFlights : FlightServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await service.GetUserFlights(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfUserHasNoFlights()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetUserFlightsAsync(It.IsAny<User>())).ReturnsAsync(new List<Entity.UserFlight>());

                var result = await service.GetUserFlights(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserFlightsAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUserHasFlights()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetUserFlightsAsync(It.IsAny<User>())).ReturnsAsync(new List<Entity.UserFlight>() {
                    new Entity.UserFlight(),
                    new Entity.UserFlight()
                });

                var result = await service.GetUserFlights(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserFlightsAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class SearchFlight : FlightServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await service.SearchFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfFlightNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync((FlightsResponse)null);

                var result = await service.SearchFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfFlightDateNotValid()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now.AddDays(-2),
                           Flight = new Flight()
                           {
                               Iata = "FLIGHTNO",
                           },
                           FlightStatus = "Active",
                       }
                    }
                });

                var result = await service.SearchFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfFlightDateValid()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                aviationstackFlightService.Setup(asfs => asfs.GetFlight(It.IsAny<string>())).ReturnsAsync(new FlightsResponse()
                {
                    Data = new List<Data>() {
                       new Data()
                       {
                           FlightDate = DateTimeOffset.Now,
                           Flight = new Flight()
                           {
                               Iata = "FLIGHTNO",
                           },
                           FlightStatus = "Active",
                       }
                    }
                });

                var result = await service.SearchFlight(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                aviationstackFlightService.Verify(asfs => asfs.GetFlight(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }
    }
}