using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IWeatherService
    {
        public Task<Result<WeatherDTO>> GetWeatherByIcao(string icao, ClaimsPrincipal claims);
    }
}