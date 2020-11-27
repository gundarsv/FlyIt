using FlyIt.Domain.Models.StationResponse;
using FlyIt.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class StationServiceTest
    {
        private readonly Mock<HttpMessageHandler> handlerMock;
        private readonly ICheckWXAPIStationService checkWXAPIStationService;
        private readonly Mock<ILogger<CheckWXAPIStationService>> logger;

        public StationServiceTest()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };

            logger = new Mock<ILogger<CheckWXAPIStationService>>();

            checkWXAPIStationService = new CheckWXAPIStationService(httpClient, logger.Object);
        }

        [TestClass]
        public class GetStationByICAO : StationServiceTest
        {
            [TestMethod]
            public async Task ReturnsNullIfNotSuccessStatusCode()
            {
                handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                })
                .Verifiable();

                var result = await checkWXAPIStationService.GetStationByICAO(It.IsAny<string>());

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsNullIfDataIsEmpty()
            {
                handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent("{'results': 0,'data': []}"),
                    StatusCode = HttpStatusCode.OK,
                })
                .Verifiable();

                var result = await checkWXAPIStationService.GetStationByICAO(It.IsAny<string>());

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsStationResponse()
            {
                handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent("{'results':1,'data':[{'country':{'code':'LV','name':'Latvia'},'location':{'coordinates':[23.9711,56.923599],'type':'Point'},'latitude':{'decimal':56.923599,'degrees':'56° 55 24.95 N'},'longitude':{'decimal':23.9711,'degrees':'23° 58 15.95 E'},'timezone':{'tzid':'Europe/Riga','gmt':200,'zone':'EET','dst':false},'elevation':{'feet':36,'meters':11},'icao':'EVRA','iata':'RIX','name':'Riga International Airport','type':'Airport','city':'Riga','status':'Operational'}]}"),
                    StatusCode = HttpStatusCode.OK,
                })
                .Verifiable();

                var result = await checkWXAPIStationService.GetStationByICAO(It.IsAny<string>());

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(StationResponse));
            }

            [TestMethod]
            public async Task ReturnsNullIfThrowsException()
            {
                handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception())
                .Verifiable();

                var result = await checkWXAPIStationService.GetStationByICAO(It.IsAny<string>());

                Assert.IsNull(result);
            }
        }
    }
}