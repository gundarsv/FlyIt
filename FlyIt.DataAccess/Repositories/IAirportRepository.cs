using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IAirportRepository
    {
        public Task<List<Airport>> GetAirportsAsync();

        public Airport AddAirport(Airport airport);

        public Airport GetAirportById(int id);

        public Task<Airport> GetAirportByIdAsync(int id);

        public UserAirport AddUserAirport(User user, Airport airport);

        public Task<UserAirport> AddUserAirportAsync(User user, Airport airport);

        public Airport UpdateAirport(int id, Airport airport);

        public List<UserAirport> GetUserAirports(User user);

        public Airport GetAirportByIata(string iata);

        public UserAirport GetUserAirport(User user, Airport airport);

        public UserAirport GetUserAirportById(User user, int id);

        public UserAirport RemoveUserAirport(UserAirport userAirport);
    }
}