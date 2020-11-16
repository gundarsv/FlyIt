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
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class AirportServiceTest
    {
        private readonly IMapper mapper;
        private readonly Mock<IAirportRepository> repository;
        private readonly Mock<UserManager<User>> userManager;
        private readonly AirportService airportService;
        private readonly Mock<IGoogleCloudStorageService> googleCloudStorageService;

        public AirportServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();

            this.userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            this.repository = new Mock<IAirportRepository>();
            var airportMappingProfile = new AirportMapping();

            this.googleCloudStorageService = new Mock<IGoogleCloudStorageService>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(airportMappingProfile));
            mapper = new Mapper(configuration);

            airportService = new AirportService(mapper, repository.Object, userManager.Object, googleCloudStorageService.Object);
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
                repository.Setup(repository => repository.GetUserAirportsAsync(It.IsAny<User>())).ReturnsAsync(airports);

                var result = await airportService.GetUserAirports(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserAirportsAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFound()
            {
                List<UserAirport> airports = new List<UserAirport>();

                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);
                repository.Setup(repository => repository.GetUserAirportsAsync(It.IsAny<User>())).ReturnsAsync(airports);

                var result = await airportService.GetUserAirports(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserAirportsAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsExceptionAsync()
            {
                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);
                repository.Setup(repository => repository.GetUserAirportsAsync(It.IsAny<User>())).ThrowsAsync(new Exception());

                var result = await airportService.GetUserAirports(It.IsAny<ClaimsPrincipal>());

                userManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetUserAirportsAsync(It.IsAny<User>()), Times.Once);
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
            public async Task ReturnsInvalidIfRolesReturnsNull()
            {
                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotInAirportsAdministratorRole()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator"
                };

                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportIsNull()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserAirportExists()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRepositoryReturnsNull()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserAirport)null);
                repository.Setup(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsAirport()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserAirport)null);
                repository.Setup(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ReturnsAsync(new UserAirport());

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Created, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                userManager.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(repository => repository.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ThrowsAsync(new Exception());

                var result = await airportService.AddAirportToUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(m => m.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class RemoveAirportFromUser : AirportServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await airportService.RemoveAirportFromUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await airportService.RemoveAirportFromUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfUserAirportNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.RemoveAirportFromUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotDeleted()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveUserAirportAsync(It.IsAny<UserAirport>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.RemoveAirportFromUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.RemoveUserAirportAsync(It.IsAny<UserAirport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveUserAirportAsync(It.IsAny<UserAirport>())).ThrowsAsync(new Exception());

                var result = await airportService.RemoveAirportFromUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.RemoveUserAirportAsync(It.IsAny<UserAirport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsDeletedUserAirport()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveUserAirportAsync(It.IsAny<UserAirport>())).ReturnsAsync(new UserAirport());

                var result = await airportService.RemoveAirportFromUser(It.IsAny<int>(), It.IsAny<int>());

                userManager.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(r => r.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(r => r.RemoveUserAirportAsync(It.IsAny<UserAirport>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class AddAirport : AirportServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserNotInRole()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportExists()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(new Airport());

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportNotCreated()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync((Airport)null);
                repository.Setup(r => r.AddAirportAsync(It.IsAny<Airport>())).ReturnsAsync((Airport)null);

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.AddAirportAsync(It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportNotAddedToUser()
            {
                List<string> roles = new List<string>()
                {
                    "SystemAdministrator",
                    "AirportsAdministrator"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync((Airport)null);
                repository.Setup(r => r.AddAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.AddAirportAsync(It.IsAny<Airport>()), Times.Once);
                repository.Verify(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
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

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync((Airport)null);
                repository.Setup(r => r.AddAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ReturnsAsync(new UserAirport());

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.AddAirportAsync(It.IsAny<Airport>()), Times.Once);
                repository.Verify(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Created, result.ResultType);
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
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync((Airport)null);
                repository.Setup(r => r.AddAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>())).ThrowsAsync(new Exception());

                var result = await airportService.AddAirport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.AddAirportAsync(It.IsAny<Airport>()), Times.Once);
                repository.Verify(repository => repository.AddUserAirportAsync(It.IsAny<User>(), It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteAirport : AirportServiceTest
        {
            private readonly List<string> roles = new List<string>()
            {
                    "SystemAdministrator",
                    "AirportsAdministrator"
            };

            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserNotInRole()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportNotAssignedToUser()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportNotDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveAirportAsync(It.IsAny<Airport>())).ReturnsAsync((Airport)null);

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.RemoveAirportAsync(It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfImageNotDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport());
                googleCloudStorageService.Setup(gc => gc.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new UnexpectedResult<string>(It.IsAny<string>()));

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.RemoveAirportAsync(It.IsAny<Airport>()), Times.Once);
                googleCloudStorageService.Verify(googleCloudStorageService => googleCloudStorageService.DeleteFileAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveAirportAsync(It.IsAny<Airport>())).ThrowsAsync(new Exception());

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.RemoveAirportAsync(It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfAirportDeleted()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(new Airport());
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.RemoveAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport());
                googleCloudStorageService.Setup(gc => gc.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new SuccessResult<string>(null));

                var result = await airportService.DeleteAirport(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.RemoveAirportAsync(It.IsAny<Airport>()), Times.Once);
                googleCloudStorageService.Verify(googleCloudStorageService => googleCloudStorageService.DeleteFileAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class UpdateAirport : AirportServiceTest
        {
            private readonly List<string> roles = new List<string>()
            {
                    "SystemAdministrator",
                    "AirportsAdministrator"
            };

            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfRoleIsNull()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync((IList<string>)null);

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserNotInRole()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>());

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync((Airport)null);

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportWithIataAlreadyExistsAndIsNotTheSameAirport()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(new Airport());

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotAssignedToUser()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserAirport)null);

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfAirportNotUpdated()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.UpdateAirportAsync(It.IsAny<Airport>())).ReturnsAsync((Airport)null);

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.UpdateAirportAsync(It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedThrowsException()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.UpdateAirportAsync(It.IsAny<Airport>())).ThrowsAsync(new Exception());

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.UpdateAirportAsync(It.IsAny<Airport>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfOldMapNotDeleted()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.UpdateAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport() { MapName = "MapName" });
                googleCloudStorageService.Setup(gc => gc.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new UnexpectedResult<string>(null));

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.UpdateAirportAsync(It.IsAny<Airport>()), Times.Once);
                googleCloudStorageService.Verify(googleCloudStorageService => googleCloudStorageService.DeleteFileAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccesIfOldMapDeleted()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.UpdateAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport() { MapName = "MapName" });
                googleCloudStorageService.Setup(gc => gc.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(new SuccessResult<string>(null));

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.UpdateAirportAsync(It.IsAny<Airport>()), Times.Once);
                googleCloudStorageService.Verify(googleCloudStorageService => googleCloudStorageService.DeleteFileAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUpdatedWihtoutMapName()
            {
                var airport = new Airport()
                {
                    Iata = "BLL",
                    MapName = "Same"
                };

                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                userManager.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
                repository.Setup(r => r.GetAirportByIdAsync(It.IsAny<int>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);
                repository.Setup(r => r.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new UserAirport());
                repository.Setup(r => r.UpdateAirportAsync(It.IsAny<Airport>())).ReturnsAsync(new Airport() { MapName = "Same" });

                var result = await airportService.UpdateAirport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                userManager.Verify(userManager => userManager.GetRolesAsync(It.IsAny<User>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIdAsync(It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                repository.Verify(repository => repository.GetUserAirportByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                repository.Verify(repository => repository.UpdateAirportAsync(It.IsAny<Airport>()), Times.Once);
                googleCloudStorageService.Verify(googleCloudStorageService => googleCloudStorageService.DeleteFileAsync(It.IsAny<string>()), Times.Never);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestClass]
        public class GetAirportByIata : AirportServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await airportService.GetAirportByIata(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfAirportNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync((Airport)null);

                var result = await airportService.GetAirportByIata(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);
                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccesIfAirportExists()
            {
                Airport airport = new Airport();
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
                repository.Setup(r => r.GetAirportByIataAsync(It.IsAny<string>())).ReturnsAsync(airport);

                var result = await airportService.GetAirportByIata(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>());

                userManager.Verify(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                repository.Verify(r => r.GetAirportByIataAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }
    }
}