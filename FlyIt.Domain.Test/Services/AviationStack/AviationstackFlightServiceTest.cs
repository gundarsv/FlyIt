using FlyIt.Domain.Models.AviationstackResponses;
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
    public class AviationstackFlightServiceTest
    {
        private readonly Mock<HttpMessageHandler> handlerMock;
        private readonly AviationstackFlightService aviationstackFlightService;
        private readonly Mock<ILogger<AviationstackFlightService>> logger;

        public AviationstackFlightServiceTest()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };

            logger = new Mock<ILogger<AviationstackFlightService>>();

            aviationstackFlightService = new AviationstackFlightService(httpClient, logger.Object);
        }

        [TestClass]
        public class GetFlight : AviationstackFlightServiceTest
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

                var result = await aviationstackFlightService.GetFlight(It.IsAny<string>());

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
                    Content = new StringContent("{'pagination':{'limit':100,'offset':0,'count':0,'total':0},'data':[]}"),
                    StatusCode = HttpStatusCode.OK,
                })
                .Verifiable();

                var result = await aviationstackFlightService.GetFlight(It.IsAny<string>());

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsFlightResponse()
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
                    Content = new StringContent("{'pagination':{'limit':100,'offset':0,'count':1,'total':1},'data':[{'flight_date':'2020-11-16','flight_status':'landed','departure':{'airport':'Billund','timezone':'Europe/Copenhagen','iata':'BLL','icao':'EKBI','terminal':null,'gate':'4','delay':null,'scheduled':'2020-11-16T06:55:00+00:00','estimated':'2020-11-16T06:55:00+00:00','actual':'2020-11-16T06:54:00+00:00','estimated_runway':'2020-11-16T06:54:00+00:00','actual_runway':'2020-11-16T06:54:00+00:00'},'arrival':{'airport':'Gardermoen','timezone':'Europe/Oslo','iata':'OSL','icao':'ENGM','terminal':'5','gate':null,'baggage':'6','delay':null,'scheduled':'2020-11-16T08:05:00+00:00','estimated':'2020-11-16T08:05:00+00:00','actual':'2020-11-16T07:51:00+00:00','estimated_runway':'2020-11-16T07:51:00+00:00','actual_runway':'2020-11-16T07:51:00+00:00'},'airline':{'name':'SAS','iata':'SK','icao':'SAS'},'flight':{'number':'1900','iata':'SK1900','icao':'SAS1900','codeshared':null},'aircraft':null,'live':null}]}"),
                    StatusCode = HttpStatusCode.OK,
                })
                .Verifiable();

                var result = await aviationstackFlightService.GetFlight(It.IsAny<string>());

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(FlightsResponse));
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

                var result = await aviationstackFlightService.GetFlight(It.IsAny<string>());

                Assert.IsNull(result);
            }
        }
    }
}