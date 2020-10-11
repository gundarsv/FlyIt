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
    public class AirportController : ControllerBase
    {
        private readonly IAirportService airportService;

        public AirportController(IAirportService airportService)
        {
            this.airportService = airportService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await airportService.GetUserAirports(User);

            return this.FromResult(result);
        }

        [HttpGet("Search/{airportId}")]
        public async Task<IActionResult> Search(int id)
        {
            var result = await airportService.SearchAirport(id);

            return this.FromResult(result);
        }

        [HttpGet("{airportId}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await airportService.GetFlight(User, id);

            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAirport(AirportSearchDTO airport)
        {
            var result = await airportService.AddAirport(User, airport);

            return this.FromResult(result);
        }

        [HttpDelete("{airportId}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var result = await airportService.DeleteFlight(User, id);

            return this.FromResult(result);
        }
    }
}
