using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class NewsRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly NewsRepository newsRepository;

        public NewsRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
            .UseInMemoryDatabase(databaseName: "FlyIt-News")
            .Options;

            flyItContext = new FlyItContext(options);

            newsRepository = new NewsRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.Airport);
            flyItContext.RemoveRange(flyItContext.News);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class GetNewsAsync : NewsRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnNewsByAirportId()
            {
                var airport = new Airport();

                List<News> news = new List<News>()
                {
                    new News()
                    {
                        Title="Large queues at Copenhagen airport",
                        Imageurl="thisisateststring",
                        Body="Please come earlier at the airport if you need to do the check in on the spot",
                        AirportId=airport.Id,
                        Airport=airport
                    },
                    new News()
                    {
                        Title="Billund Airport Sales",
                        Imageurl="thisisanotherteststring",
                        Body="Big sales on Lego Toys at Billund airport",
                        AirportId=airport.Id,
                        Airport=airport
                    }
                };

                await flyItContext.Airport.AddAsync(airport);

                await flyItContext.SaveChangesAsync(default);

                await flyItContext.News.AddRangeAsync(news);

                await flyItContext.SaveChangesAsync(default);

                var result = await newsRepository.GetNewsByAirportIdAsync(airport.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(news.Count, result.Count);
            }
        }

        [TestClass]
        public class GetNewsById : NewsRepositoryTest
        {
            [TestMethod]
            public async Task CanReturnNewsById()
            {
                var airport = new Airport();

                var newsItem = new News()
                {
                    Title = "Big News in Billund Airport",
                    Imageurl = "teststring",
                    Body = "Lorem ipsum",
                    AirportId = airport.Id,
                    Airport = airport
                };

                List<News> news = new List<News>()
                {
                    new News()
                    {
                        Title="Large queues at Copenhagen airport",
                        Imageurl="thisisateststring",
                        Body="Please come earlier at the airport if you need to do the check in on the spot",
                        AirportId = airport.Id,
                        Airport = airport
                    },
                    new News()
                    {
                        Title="Billund Airport Sales",
                        Imageurl="thisisanotherteststring",
                        Body="Big sales on Lego Toys at Billund airport",
                        AirportId = airport.Id,
                        Airport = airport
                    },
                    newsItem,
                };
                await flyItContext.Airport.AddAsync(airport);

                await flyItContext.SaveChangesAsync();

                await flyItContext.News.AddRangeAsync(news);

                await flyItContext.SaveChangesAsync();

                var result = await newsRepository.GetNewsByIdAsync(newsItem.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(newsItem.Id, result.Id);
                Assert.AreEqual(newsItem.Title, result.Title);
                Assert.AreEqual(newsItem.Imageurl, result.Imageurl);
                Assert.AreEqual(newsItem.Body, result.Body);
                Assert.AreEqual(newsItem.AirportId, result.AirportId);
            }

            [TestMethod]
            public async Task ReturnsNullIfNotFound()
            {
                var result = await newsRepository.GetNewsByIdAsync(1);

                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class AddNewsAsync : NewsRepositoryTest
        {
            [TestMethod]
            public async Task CanAddNews()
            {
                var airport = new Airport();

                var newsItemTest = new News()
                {
                    Title = "Big News in Billund Airport",
                    Imageurl = "teststring",
                    Body = "Lorem ipsum",
                    AirportId = airport.Id,
                    Airport = airport,
                };

                await flyItContext.Airport.AddAsync(airport);

                await flyItContext.SaveChangesAsync(default);

                var result = await newsRepository.AddNewsAsync(newsItemTest);

                await flyItContext.News.AddRangeAsync(newsItemTest);

                Assert.IsNotNull(result);
                Assert.AreEqual(newsItemTest.Id, result.Id);
                Assert.AreEqual(newsItemTest.Title, result.Title);
                Assert.AreEqual(newsItemTest.Imageurl, result.Imageurl);
                Assert.AreEqual(newsItemTest.Body, result.Body);
                Assert.AreEqual(newsItemTest.AirportId, result.AirportId);
            }
        }

        [TestClass]
        public class RemoveNewsAsync : NewsRepositoryTest
        {
            [TestMethod]
            public async Task CanRemoveNews()
            {
                var airport = new Airport();

                var newsItemTestRemove = new News()
                {
                    Title = "Big News in Billund Airport",
                    Imageurl = "teststring",
                    Body = "Lorem ipsum",
                    AirportId = airport.Id,
                    Airport = airport,
                };

                await flyItContext.Airport.AddAsync(airport);

                await flyItContext.SaveChangesAsync();

                await flyItContext.News.AddAsync(newsItemTestRemove);

                await flyItContext.SaveChangesAsync();

                var resultBeforeRemomving = await flyItContext.News.SingleOrDefaultAsync(newsItemTestRemove => newsItemTestRemove.Id == newsItemTestRemove.Id);
                var result = await newsRepository.RemoveNewsAsync(newsItemTestRemove);
                var resultAfterRemoving = await flyItContext.News.SingleOrDefaultAsync(newsItemTestRemove => newsItemTestRemove.Id == newsItemTestRemove.Id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(resultBeforeRemomving);
                Assert.IsNull(resultAfterRemoving);
            }
        }

        [TestClass]
        public class UpdateNewsAsync : NewsRepositoryTest
        {
            [TestMethod]
            public async Task CanUpdateNews()
            {
                var airport = new Airport();

                var newsItemTestUpdate = new News()
                {
                    Title = "Big News in Billund Airport",
                    Imageurl = "teststring",
                    Body = "Lorem ipsum",
                    AirportId = airport.Id,
                    Airport = airport,
                };

                await flyItContext.Airport.AddAsync(airport);

                await flyItContext.SaveChangesAsync(default);

                await flyItContext.News.AddAsync(newsItemTestUpdate);

                await flyItContext.SaveChangesAsync();

                var updatedNews = new News()
                {
                    Id = newsItemTestUpdate.Id,
                    Title = "Big News in Billund Airport",
                    Imageurl = "teststring",
                    Body = "AriciPogonici",
                    AirportId = airport.Id,
                    Airport = airport,
                };

                var resultBeforeUpdating = await flyItContext.News.SingleOrDefaultAsync(newsItemTestUpdate => newsItemTestUpdate.Id == newsItemTestUpdate.Id);
                var result = await newsRepository.UpdateNewsAsync(updatedNews);
                var resultAfterUpdating = await flyItContext.News.SingleOrDefaultAsync(newsItemTestUpdate => newsItemTestUpdate.Id == newsItemTestUpdate.Id);

                Assert.IsNotNull(result);
                Assert.AreEqual(resultBeforeUpdating.Id, newsItemTestUpdate.Id);
                Assert.AreEqual(resultBeforeUpdating.Title, newsItemTestUpdate.Title);
                Assert.AreEqual(resultBeforeUpdating.Imageurl, newsItemTestUpdate.Imageurl);
                Assert.AreEqual(resultBeforeUpdating.Body, newsItemTestUpdate.Body);
                Assert.AreEqual(resultBeforeUpdating.AirportId, newsItemTestUpdate.AirportId);
            }
        }
    }
}