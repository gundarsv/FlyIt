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
    public class NewsServiceTest
    {
        private readonly IMapper mapper;
        private readonly Mock<IAirportRepository> airportRepository;
        private readonly Mock<INewsRepository> newsRepository;
        private readonly Mock<UserManager<User>> userManager;
        private readonly NewsService newsService;
        private readonly Mock<IGoogleCloudStorageService> googlecloudStorageService;

        private readonly List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

        public NewsServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            this.airportRepository = new Mock<IAirportRepository>();
            var airportMappingProfile = new AirportMapping();

            this.newsRepository = new Mock<INewsRepository>();
            var newsMappingProfile = new NewsMapping();

            this.googlecloudStorageService = new Mock<IGoogleCloudStorageService>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(newsMappingProfile));
            mapper = new Mapper(configuration);

            newsService = new NewsService(userManager.Object, airportRepository.Object, newsRepository.Object, mapper, googlecloudStorageService.Object);
        }

        [TestClass]
        public class GetNewsByAiportId : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsSuccessIfNewsExist()
            {
                List<News> news = new List<News>()
                {
                    new News(), new News()
                };

                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.GetNewsByAirportId(It.IsAny<int>());

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfNewsCountLessThan1()
            {
                List<News> news = new List<News>();

                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.GetNewsByAirportId(It.IsAny<int>());

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfNewsByAirportIdNotFound()
            {
                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await newsService.GetNewsByAirportId(It.IsAny<int>());

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

                var result = await newsService.GetNewsByAirportId(It.IsAny<int>());

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class GetNewsByAiportIata : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfNewsCountLessThan1()
            {
                List<News> news = new List<News>();

                airportRepository.Setup(a => a.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.GetNewsByAirportIata(It.IsAny<string>());

                airportRepository.Verify(a => a.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                airportRepository.Setup(a => a.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync((Airport)null);

                var result = await newsService.GetNewsByAirportIata(It.IsAny<string>());

                airportRepository.Verify(a => a.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfGetNewsByAirportIdReturnsNull()
            {
                airportRepository.Setup(a => a.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync((List<News>)null);

                var result = await newsService.GetNewsByAirportIata(It.IsAny<string>());

                airportRepository.Verify(a => a.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                airportRepository.Setup(a => a.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

                var result = await newsService.GetNewsByAirportIata(It.IsAny<string>());

                airportRepository.Verify(a => a.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfNewsExist()
            {
                List<News> news = new List<News>()
                {
                    new News(), new News()
                };

                airportRepository.Setup(a => a.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.GetNewsByAirportIata(It.IsAny<string>());

                airportRepository.Verify(a => a.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class AddNews : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNewsNotCreated()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                airportRepository.Setup(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.AddNewsAsync(It.IsAny<News>())).ReturnsAsync((News)null);

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.AddNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfNewsCreated()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                airportRepository.Setup(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.AddNewsAsync(It.IsAny<News>())).ReturnsAsync(new News());

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.AddNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteNews : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserNotInRole()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfNewsNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync((News)null);

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNewsNotDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ReturnsAsync((News)null);

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfOldImageIsNotDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ReturnsAsync(new News());
                googlecloudStorageService.Setup(gs => gs.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new UnexpectedResult<string>());

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);
                googlecloudStorageService.Verify(gs => gs.DeleteFileAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ThrowsAsync(new Exception());

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfNewsDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ReturnsAsync(new News());
                googlecloudStorageService.Setup(gs => gs.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new SuccessResult<string>(It.IsAny<string>()));

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);
                googlecloudStorageService.Verify(gs => gs.DeleteFileAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class UpdateNews : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserNotInRole()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfNewsNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync((News)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNewsNotUpdated()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                airportRepository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ReturnsAsync((News)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.UpdateNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUpdatedAndImageNameIsTheSame()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News() { ImageName = "name" });
                airportRepository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ReturnsAsync(new News() { ImageName = "name" });

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.UpdateNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfImageNotDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News() { ImageName = "value" });
                airportRepository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ReturnsAsync(new News() { ImageName = "differentValue" });
                googlecloudStorageService.Setup(gs => gs.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new UnexpectedResult<string>(It.IsAny<string>()));

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.UpdateNewsAsync(It.IsAny<News>()), Times.Once);
                googlecloudStorageService.Verify(gs => gs.DeleteFileAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ThrowsAsync(new Exception());

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUpdatedAndImageDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News() { ImageName = "value" });
                airportRepository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ReturnsAsync(new News() { ImageName = "differentValue" });
                googlecloudStorageService.Setup(gs => gs.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new SuccessResult<string>(It.IsAny<string>()));

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.UpdateNewsAsync(It.IsAny<News>()), Times.Once);
                googlecloudStorageService.Verify(gs => gs.DeleteFileAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }
    }
}