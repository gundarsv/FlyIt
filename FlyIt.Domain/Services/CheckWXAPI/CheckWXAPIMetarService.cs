using FlyIt.Domain.Models.MetarResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class CheckWXAPIMetarService : ICheckWXAPIMetarService
    {
        private readonly HttpClient httpClient;

        public CheckWXAPIMetarService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<MetarResponse> GetMetarByICAO(string icao)
        {
            try
            {
                var responseString = await httpClient.GetStringAsync("metar/" + icao + "/decoded");

                var metarResponse = JsonConvert.DeserializeObject<MetarResponse>(responseString);

                if (metarResponse.Data.Length < 1)
                {
                    return null;
                }

                return metarResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}