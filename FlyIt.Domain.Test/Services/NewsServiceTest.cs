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
        }
    }
}