using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IAirportService
    {
        public Task<Result<List<AirportDTO>>> GetUserAirports(ClaimsPrincipal claims);

        public Task<Result<List<AirportDTO>>> GetAllAirports();

        public Task<Result<AirportDTO>> GetAirportByIata(string Iata, ClaimsPrincipal claims);

        public Task<Result<AirportDTO>> AddAirportToUser(int aiportId, int userId);

        public Task<Result<AirportDTO>> RemoveAirportFromUser(int airportId, int userId);

        public Task<Result<AirportDTO>> AddAirport(string Iata, string Name, string MapUrl, string MapName, string RentingCompanyUrl, string RentingCompanyName, int RentingCompanyPhoneNo, ClaimsPrincipal claims);

        public Task<Result<AirportDTO>> DeleteAirport(int id, ClaimsPrincipal claims);

        public Task<Result<AirportDTO>> UpdateAirport(int id, string Iata, string Name, string MapUrl, string MapName, string RentingCompanyUrl, string RentingCompanyName, int RentingCompanyPhoneNo, ClaimsPrincipal claims);
    }
}