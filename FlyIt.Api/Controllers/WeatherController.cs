using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyIt.Domain.Models.MetarResponse;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlyIt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly ICheckWXAPIMetarService checkWXAPIMetarService;
        private readonly IAviationstackFlightService aviationStackFlightService;

        public WeatherController(ICheckWXAPIMetarService checkWXAPIMetarService, IAviationstackFlightService aviationStackFlightService)
        {
            this.checkWXAPIMetarService = checkWXAPIMetarService;
            this.aviationStackFlightService = aviationStackFlightService;
        }

        // GET: api/<WeatherController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WeatherController>/5
        [HttpGet("{icao}")]
        public async Task<IActionResult> Get(string icao)
        {
            var result = await checkWXAPIMetarService.GetMetarByICAO(icao);

            return Ok(result);
        }

        // POST api/<WeatherController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WeatherController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WeatherController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}