using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyIt.Api.Extensions;
using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            this.weatherService = weatherService;
        }

        [HttpGet("{icao}")]
        public async Task<IActionResult> Get(string icao)
        {
            var result = await weatherService.GetWeatherByIcao(icao, User);

            return this.FromResult(result);
        }
    }
}