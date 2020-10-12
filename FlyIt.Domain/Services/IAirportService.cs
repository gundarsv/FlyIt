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
    }
}