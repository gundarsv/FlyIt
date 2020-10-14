using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IAirportRepository
    {
        public Task<List<Airport>> GetAirportsAsync();

        public Task<Airport> AddAirportAsync(Airport airport);

        public Task<Airport> GetAirportByIdAsync(int id);

        public Task<UserAirport> AddUserAirportAsync(User user, Airport airport);

        public Task<Airport> UpdateAirportAsync(Airport airport);

        public Task<List<UserAirport>> GetUserAirportsAsync(User user);

        public Task<Airport> RemoveAirportAsync(Airport airport);

        public Task<Airport> GetAirportByIataAsync(string iata);

        public Task<UserAirport> GetUserAirportByIdAsync(int userId, int airportId);

        public Task<UserAirport> RemoveUserAirportAsync(UserAirport userAirport);
    }
}