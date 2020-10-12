﻿using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class AirportServiceTest
    {
        private readonly IMapper mapper;
        private readonly Mock<IAirportRepository> repository;
        private readonly Mock<UserManager<User>> userManager;
        private readonly AirportService airportService;

        public AirportServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            this.repository = new Mock<IAirportRepository>();
            var airportMappingProfile = new AirportMapping();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(airportMappingProfile));
            mapper = new Mapper(configuration);

            airportService = new AirportService(mapper, repository.Object, userManager.Object);
        }

        [TestClass]
        public class GetUserAirports : AirportServiceTest
        {
            [TestMethod]
            public async Task ReturnsAirports()
            {
                List<UserAirport> airports = new List<UserAirport>()
                {
                    new UserAirport(), new UserAirport()
                };

                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);
                repository.Setup(repository => repository.GetUserAirports(It.IsAny<User>())).Returns(airports);

                var result = await airportService.GetUserAirports(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserAirports(It.IsAny<User>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFound()
            {
                List<UserAirport> airports = new List<UserAirport>();

                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);
                repository.Setup(repository => repository.GetUserAirports(It.IsAny<User>())).Returns(airports);

                var result = await airportService.GetUserAirports(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserAirports(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);
                repository.Setup(repository => repository.GetUserAirports(It.IsAny<User>())).Throws(new Exception());

                var result = await airportService.GetUserAirports(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserAirports(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class GetAllAirports : AirportServiceTest
        {
            [TestMethod]
            public async Task ReturnsAirports()
            {
                List<Airport> airports = new List<Airport>()
                {
                    new Airport(), new Airport()
                };

                repository.Setup(repository => repository.GetAirportsAsync()).ReturnsAsync(airports);

                var result = await airportService.GetAllAirports();

                repository.Verify(r => r.GetAirportsAsync(), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfNoAirports()
            {
                List<Airport> airports = new List<Airport>();

                repository.Setup(repository => repository.GetAirportsAsync()).ReturnsAsync(airports);

                var result = await airportService.GetAllAirports();

                repository.Verify(r => r.GetAirportsAsync(), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                repository.Setup(repository => repository.GetAirportsAsync()).ThrowsAsync(new Exception());

                var result = await airportService.GetAllAirports();

                repository.Verify(r => r.GetAirportsAsync(), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class AddAirportToUser : AirportServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserIsNull()
            {
                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportIsNull()
            {
                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRepositoryReturnsNull()
            {
                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsAirport()
            {
                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ReturnsAsync(new UserAirport());

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Created, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ThrowsAsync(new Exception());

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }
    }
}