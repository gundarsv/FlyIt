using System.Threading.Tasks;
using FlyIt.Api.Extensions;
using FlyIt.Domain.Models;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService flightService;

        public FlightController(IFlightService flightService)
        {
            this.flightService = flightService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await flightService.GetUserFlights(User);

            return this.FromResult(result);
        }

        [HttpGet("Search/{flightNo}")]
        public async Task<IActionResult> Search(string flightNo)
        {
            var result = await flightService.SearchFlight(flightNo);

            return this.FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await flightService.GetFlight(User, id);

            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddFlight(FlightSearchDTO flight)
        {
            var result = await flightService.AddFlight(User, flight);

            return this.FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var result = await flightService.DeleteFlight(User, id);

            return this.FromResult(result);
        }
    }
}
