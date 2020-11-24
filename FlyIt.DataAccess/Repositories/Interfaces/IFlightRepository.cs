using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IFlightRepository
    {
        public Task<Flight> AddFlightAsync(Flight flight);

        public Task<Flight> GetFlightByIdAsync(int id);

        public Task<UserFlight> AddUserFlightAsync(User user, Flight flight);

        public Task<Flight> UpdateFlightAsync(Flight flight);

        public Task<List<UserFlight>> GetUserFlightsAsync(User user);

        public Task<Flight> GetFlightByDateAndFlightNumberAsync(DateTimeOffset date, string flightNo);

        public Task<UserFlight> GetUserFlightByIdAsync(int userId, int flightId);

        public Task<UserFlight> RemoveUserFlightAsync(UserFlight userFlight);
    }
}