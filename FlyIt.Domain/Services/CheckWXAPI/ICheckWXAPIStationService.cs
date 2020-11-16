using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.Models.StationResponse;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface ICheckWXAPIStationService
    {
        public Task<StationResponse> GetStationByICAO(string icao);
    }
}