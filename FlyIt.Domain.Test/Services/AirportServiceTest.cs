using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

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
        public class GetAirports : AirportServiceTest
        {
        }

    }
}
