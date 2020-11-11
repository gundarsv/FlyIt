using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.Models.StationResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class CheckWXAPIStationService : ICheckWXAPIStationService
    {
        private readonly HttpClient httpClient;

        public CheckWXAPIStationService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<StationResponse> GetStationByICAO(string icao)
        {
            try
            {
                var responseString = await httpClient.GetStringAsync("station/" + icao);

                var metarResponse = JsonConvert.DeserializeObject<StationResponse>(responseString);

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