using FlyIt.Domain.Models.MetarResponse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class CheckWXAPIMetarService : ICheckWXAPIMetarService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<CheckWXAPIMetarService> logger;

        public CheckWXAPIMetarService(HttpClient httpClient, ILogger<CheckWXAPIMetarService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<MetarResponse> GetMetarByICAO(string icao)
        {
            try
            {
                var response = await httpClient.GetAsync("metar/" + icao + "/decoded");

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"GetMetarByICAO Failed + {icao}", response);
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();

                var metarResponse = JsonConvert.DeserializeObject<MetarResponse>(responseString);

                if (metarResponse.Data.Count < 1)
                {
                    return null;
                }

                return metarResponse;
            }
            catch (Exception ex)
            {
                logger.LogError("GetMetarByICAO threw: ", ex);
                return null;
            }
        }
    }
}