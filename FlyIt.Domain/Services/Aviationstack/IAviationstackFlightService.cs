using FlyIt.Domain.Models.AviationstackResponses;
using FlyIt.Domain.ServiceResult;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IAviationstackFlightService
    {
        public Task<Result<FlightsResponse>> GetFlight(string flightNo);
    }
}
