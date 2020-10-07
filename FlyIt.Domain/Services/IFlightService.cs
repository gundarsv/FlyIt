using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IFlightService
    {
        public Task<Result<FlightSearchDTO>> SearchFlight(string flightNo);

        public Task<Result<FlightDTO>> GetFlight(ClaimsPrincipal claims, int id);

        public Task<Result<FlightDTO>> AddFlight(ClaimsPrincipal claims, FlightSearchDTO flightDTO);

        public Task<Result<string>> DeleteFlight(ClaimsPrincipal claims, int id);

        public Task<Result<List<FlightDTO>>> GetUserFlights(ClaimsPrincipal claims);
    }
}
