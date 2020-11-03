using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly FlyItContext context;

        public FlightRepository(FlyItContext context)
        {
            this.context = context;
        }

        public async Task<Flight> AddFlightAsync(Flight flight)
        {
            var entityEntry = await context.Flight.AddAsync(flight);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return entityEntry.Entity;
        }

        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            return await context.Flight.SingleOrDefaultAsync(flight => flight.Id == id);
        }

        public async Task<UserFlight> AddUserFlightAsync(User user, Flight flight)
        {
            if (user is null || flight is null)
            {
                return null;
            }

            var userFlight = new UserFlight
            {
                User = user,
                Flight = flight
            };

            var entityEntry = await context.UserFlight.AddAsync(userFlight);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return entityEntry.Entity;
        }

        public async Task<List<UserFlight>> GetUserFlightsAsync(User user)
        {
            return await context.UserFlight.Include(uf => uf.Flight).Where(uf => uf.UserId == user.Id).ToListAsync();
        }

        public async Task<Flight> GetFlightByDateAndFlightNumberAsync(DateTimeOffset date, string flightNo)
        {
            return await context.Flight.SingleOrDefaultAsync(f => f.FlightNo == flightNo && f.Date == date);
        }

        public async Task<UserFlight> RemoveUserFlightAsync(UserFlight userFlight)
        {
            var removedUserFlight = context.UserFlight.Remove(userFlight);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return removedUserFlight.Entity;
        }

        public async Task<UserFlight> GetUserFlightByIdAsync(int userId, int flightId)
        {
            return await context.UserFlight.Include(uf => uf.Flight).SingleOrDefaultAsync(uf => uf.FlightId == flightId && uf.UserId == userId);
        }

        public async Task<Flight> UpdateFlightAsync(Flight flight)
        {
            var flightToUpdate = await context.Flight.SingleOrDefaultAsync(f => f.Id == flight.Id);

            if (flightToUpdate is null)
            {
                return null;
            }

            context.Entry(flightToUpdate).CurrentValues.SetValues(flight);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return await context.Flight.SingleOrDefaultAsync(f => f.Id == flightToUpdate.Id);
        }
    }
}