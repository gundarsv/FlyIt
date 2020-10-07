using FlyIt.Domain.Models.AviationstackResponses;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IAviationstackFlightService
    {
        public Task<FlightsResponse> GetFlight(string flightNo);
    }
}
