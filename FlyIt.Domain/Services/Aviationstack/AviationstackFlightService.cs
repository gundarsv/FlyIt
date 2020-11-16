using FlyIt.Domain.Models.AviationstackResponses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class AviationstackFlightService : IAviationstackFlightService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<AviationstackFlightService> logger;

        public AviationstackFlightService(HttpClient httpClient, ILogger<AviationstackFlightService> logger)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }

        public async Task<FlightsResponse> GetFlight(string flightNo)
        {
            try
            {
                var baseAddressQuery = httpClient.BaseAddress.Query;

                var response = await httpClient.GetAsync(baseAddressQuery + "&flight_iata=" + flightNo);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"GetFlight Failed + {flightNo}", response);
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();

                var flightsResponse = JsonConvert.DeserializeObject<FlightsResponse>(responseString);

                if (flightsResponse.Data.Count < 1)
                {
                    return null;
                }

                return flightsResponse;
            }
            catch (Exception ex)
            {
                logger.LogError("GetFlight threw: ", ex);
                return null;
            }
        }
    }
}