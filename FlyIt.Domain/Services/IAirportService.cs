using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services.Aviationstack
{
    interface IAirportService
    {
        public Task<Result<AirportSearchDTO>> SearchAirport(int airportId);

        public Task<Result<AirportDTO>> GetAirport(ClaimsPrincipal claims, int id);

        public Task<Result<FlightDTO>> AddAirport(ClaimsPrincipal claims, AirportSearchDTO airportDTO);

        public Task<Result<string>> DeleteAirport(ClaimsPrincipal claims, int id);

        public Task<Result<List<FlightDTO>>> GetUserAirports(ClaimsPrincipal claims);
    }
}
