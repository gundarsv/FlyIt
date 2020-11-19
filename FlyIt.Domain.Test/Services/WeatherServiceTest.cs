using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.Models.StationResponse;
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
    public class WeatherServiceTest
    {
        private readonly IMapper mapper;
        private readonly Mock<UserManager<User>> userManager;
        private readonly WeatherService weatherService;
        private readonly Mock<ICheckWXAPIMetarService> checkWXAPIMetarService;
        private readonly Mock<ICheckWXAPIStationService> checkWXAPIStationService;

        public WeatherServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            var weatherMappingProfile = new WeatherMapping();

            checkWXAPIMetarService = new Mock<ICheckWXAPIMetarService>();

            checkWXAPIStationService = new Mock<ICheckWXAPIStationService>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(weatherMappingProfile));
            mapper = new Mapper(configuration);

            weatherService = new WeatherService(checkWXAPIMetarService.Object, userManager.Object, mapper, checkWXAPIStationService.Object);
        }

        [TestClass]
        public class GetWeatherByIcao : WeatherServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await weatherService.GetWeatherByIcao(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfMetarNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                checkWXAPIMetarService.Setup(cams => cams.GetMetarByICAO(It.IsAny<string>())).ReturnsAsync((MetarResponse)null);

                var result = await weatherService.GetWeatherByIcao(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                checkWXAPIMetarService.Verify(checkWXAPIMetarService => checkWXAPIMetarService.GetMetarByICAO(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessWithoutCity()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                checkWXAPIMetarService.Setup(cams => cams.GetMetarByICAO(It.IsAny<string>())).ReturnsAsync(new MetarResponse() { Data = new List<FlyIt.Domain.Models.MetarResponse.Datum>() { new Models.MetarResponse.Datum() { Temperature = new Dewpoint() { Celsius = 24 } } } });
                checkWXAPIStationService.Setup(cams => cams.GetStationByICAO(It.IsAny<string>())).ReturnsAsync((StationResponse)null);

                var result = await weatherService.GetWeatherByIcao(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                checkWXAPIMetarService.Verify(checkWXAPIMetarService => checkWXAPIMetarService.GetMetarByICAO(It.IsAny<string>()), Times.Once);
                checkWXAPIStationService.Verify(checkWXAPIStationService => checkWXAPIStationService.GetStationByICAO(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data.Temperature);
                Assert.IsNull(result.Data.City);
            }

            [TestMethod]
            public async Task ReturnsSuccessWithCity()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                checkWXAPIMetarService.Setup(cams => cams.GetMetarByICAO(It.IsAny<string>())).ReturnsAsync(new MetarResponse() { Data = new List<FlyIt.Domain.Models.MetarResponse.Datum>() { new Models.MetarResponse.Datum() { Temperature = new Dewpoint() { Celsius = 24 } } } });
                checkWXAPIStationService.Setup(cams => cams.GetStationByICAO(It.IsAny<string>())).ReturnsAsync(new StationResponse() { Data = new List<Models.StationResponse.Datum>() { new Models.StationResponse.Datum() { City = "Riga" } } });

                var result = await weatherService.GetWeatherByIcao(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                checkWXAPIMetarService.Verify(checkWXAPIMetarService => checkWXAPIMetarService.GetMetarByICAO(It.IsAny<string>()), Times.Once);
                checkWXAPIStationService.Verify(checkWXAPIStationService => checkWXAPIStationService.GetStationByICAO(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data.Temperature);
                Assert.IsNotNull(result.Data.City);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                checkWXAPIMetarService.Setup(cams => cams.GetMetarByICAO(It.IsAny<string>())).ReturnsAsync(new MetarResponse() { Data = new List<FlyIt.Domain.Models.MetarResponse.Datum>() { new Models.MetarResponse.Datum() { Temperature = new Dewpoint() { Celsius = 24 } } } });
                checkWXAPIStationService.Setup(cams => cams.GetStationByICAO(It.IsAny<string>())).ThrowsAsync(new Exception());

                var result = await weatherService.GetWeatherByIcao(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                checkWXAPIMetarService.Verify(checkWXAPIMetarService => checkWXAPIMetarService.GetMetarByICAO(It.IsAny<string>()), Times.Once);
                checkWXAPIStationService.Verify(checkWXAPIStationService => checkWXAPIStationService.GetStationByICAO(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }
    }
}