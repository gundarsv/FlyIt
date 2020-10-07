using FlyIt.Domain.Models.AviationstackResponses;
using FlyIt.Domain.ServiceResult;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class AviationstackFlightService : IAviationstackFlightService
    {
        private readonly HttpClient httpClient;

        public AviationstackFlightService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<FlightsResponse> GetFlight(string flightNo)
        {
            try
            {
                var baseAddressQuery = httpClient.BaseAddress.Query;
                var responseString = await httpClient.GetStringAsync(baseAddressQuery+"&flight_iata=" + flightNo);

                var flightsResponse = JsonConvert.DeserializeObject<FlightsResponse>(responseString);

                if (flightsResponse.Data.Count < 1)
                {
                    return null;
                }

                return flightsResponse;
            }
            catch
            {
                return null;
            }
        }
    }
}
