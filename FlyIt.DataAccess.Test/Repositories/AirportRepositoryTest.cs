using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
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

        [TestClass]
        public class GetAirportsAsync : AirportRepositoryTest
        {
            [TestMethod]
            public async Task ReturnsAirports()
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
    }
}
