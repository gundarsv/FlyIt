using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.Models.StationResponse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class CheckWXAPIStationService : ICheckWXAPIStationService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<CheckWXAPIStationService> logger;

        public CheckWXAPIStationService(HttpClient httpClient, ILogger<CheckWXAPIStationService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<StationResponse> GetStationByICAO(string icao)
        {
            try
            {
                var response = await httpClient.GetAsync("station/" + icao);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"GetStationByICAO Failed + {icao}", response);
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();

                var metarResponse = JsonConvert.DeserializeObject<StationResponse>(responseString);

                if (metarResponse.Data.Count < 1)
                {
                    return null;
                }

                return metarResponse;
            }
            catch (Exception ex)
            {
                logger.LogError("GetStationByICAO threw: ", ex);
                return null;
            }
        }
    }
}