using AutoMapper;
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
using System.Text;
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

        public NewsServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            this.airportRepository = new Mock<IAirportRepository>();
            var airportMappingProfile = new AirportMapping();

            this.newsRepository = new Mock<INewsRepository>();
            var newsMappingProfile = new NewsMapping();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(newsMappingProfile));
            mapper = new Mapper(configuration);

            newsService = new NewsService(userManager.Object, airportRepository.Object, newsRepository.Object, mapper);
        }

        [TestClass]
        public class GetNews : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnNews()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                List<News> news = new List<News>()
                {
                    new News(), new News()
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                airportRepository.Setup(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.GetNews(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFound()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                List<News> news = new List<News>();

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);
                airportRepository.Setup(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.GetNews(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                List<News> news = new List<News>();

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

                airportRepository.Setup(a => a.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                airportRepository.Setup(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

                var result = await newsService.GetNews(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                airportRepository.Verify(a => a.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRolesReturnsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await newsService.GetNews(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }
        }

        [TestClass]
        public class AddNews : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserIsNull()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNewsExists()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                List<News> news = new List<News>()
                {
                    new News(), new News()
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNewsNotCreated()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                List<News> news = new List<News>()
                {
                    new News(), new News()
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccess()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                List<News> news = new List<News>()
                {
                    new News(), new News()
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByAirportIdAsync(It.IsAny<int>())).ReturnsAsync(news);

                var result = await newsService.AddNews(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Created, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteNews : NewsServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

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
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>()));

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
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ReturnsAsync((News)null);

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ThrowsAsync(new Exception());

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfNewsDeleted()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.RemoveNewsAsync(It.IsAny<News>())).ReturnsAsync(new News());

                var result = await newsService.DeleteNews(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

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
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserNotInRole()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNewsNotUpdated()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ReturnsAsync(new News());

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedThrowsException()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ThrowsAsync(new Exception());

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUpdated()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                airportRepository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);
                newsRepository.Setup(n => n.GetNewsByIdAsync(It.IsAny<int>())).ReturnsAsync(new News());
                newsRepository.Setup(n => n.UpdateNewsAsync(It.IsAny<News>())).ReturnsAsync(new News());

                var result = await newsService.UpdateNews(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                airportRepository.Verify(a => a.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.GetNewsByAirportIdAsync(It.IsAny<int>()), Times.Once);
                newsRepository.Verify(n => n.RemoveNewsAsync(It.IsAny<News>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }
    }
}