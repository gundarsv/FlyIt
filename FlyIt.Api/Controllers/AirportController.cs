using FlyIt.Api.Attributes;
using FlyIt.Api.Extensions;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [AuthorizeRoles(Roles.AirportsAdministrator)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await airportService.GetUserAirports(User);

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.SystemAdministrator)]
        [HttpGet("Airports")]
        public async Task<IActionResult> GetAllAirports()
        {
            var result = await airportService.GetAllAirports();

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.SystemAdministrator)]
        [HttpPost("{airportId}/User/{userId}")]
        public async Task<IActionResult> AddAirportToUser(int airportId, int userId)
        {
            var result = await airportService.AddAirportToUser(airportId, userId);

            return this.FromResult(result);
        }

        [AuthorizeRoles(Roles.SystemAdministrator)]
        [HttpDelete("{airportId}/User/{userId}")]
        public async Task<IActionResult> RemoveAirportFromUser(int airportId, int userId)
        {
            var result = await airportService.RemoveAirportFromUser(airportId, userId);

            return this.FromResult(result);
        }
    }
}