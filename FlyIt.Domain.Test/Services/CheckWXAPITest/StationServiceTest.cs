using FlyIt.Domain.Models.StationResponse;
using FlyIt.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services.CheckWXAPITest
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
                    Content = new StringContent("{'results':1,'data':[{'wind':{'degrees':220,'speed_kts':9,'speed_mph':10,'speed_mps':5,'speed_kph':17},'temperature':{'celsius':12,'fahrenheit':54},'dewpoint':{'celsius':11,'fahrenheit':52},'humidity':{'percent':94},'barometer':{'hg':30,'hpa':1016,'kpa':101.59,'mb':1015.92},'visibility':{'miles':'Greater than 6','miles_float':6.21,'meters':'10,000+','meters_float':9999},'ceiling':{'code':'OVC','text':'Overcast','feet':1000,'meters':305,'base_feet_agl':1000,'base_meters_agl':305},'elevation':{'feet':23,'meters':7},'location':{'coordinates':[23.9711,56.923599],'type':'Point'},'icao':'EVRA','station':{'name':'Riga International Airport'},'raw_text':'EVRA 181420Z 22009KT 9999 OVC010 12/11 Q1016 R18/290195 NOSIG','observed':'2020-11-18T14:20:00.000Z','flight_category':'MVFR','clouds':[{'code':'OVC','text':'Overcast','feet':1000,'meters':305,'base_feet_agl':1000,'base_meters_agl':305}],'conditions':[]}]}"),
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