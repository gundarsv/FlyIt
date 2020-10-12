using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyIt.DataAccess.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly FlyItContext context;

        public FlightRepository(FlyItContext context)
        {
            this.context = context;
        }

        public Flight AddFlight(Flight flight)
        {
            var entityEntry = context.Flight.Add(flight);

            context.SaveChanges();

            return entityEntry.Entity;
        }

        public Flight GetFlightById(int id)
        {
            return context.Flight.SingleOrDefault(flight => flight.Id == id);
        }

        public UserFlight AddUserFlight(User user, Flight flight)
        {
            var userFlight = new UserFlight
            {
                User = user,
                Flight = flight
            };

            var entityEntry = context.UserFlight.Add(userFlight);

            context.SaveChanges();

            return entityEntry.Entity;
        }

        public List<UserFlight> GetUserFlights(User user)
        {
            return context.UserFlight.Include(uf => uf.Flight).Where(uf => uf.UserId == user.Id).ToList();
        }

        public Flight GetFlightByDateAndFlightNumber(DateTimeOffset date, string flightNo)
        {
            return context.Flight.SingleOrDefault(f => f.FlightNo == flightNo && f.Date == date);
        }

        public UserFlight RemoveUserFlight(UserFlight userFlight)
        {
            var removedUserFlight = context.UserFlight.Remove(userFlight);

            context.SaveChanges();

            return removedUserFlight.Entity;
        }

        public UserFlight GetUserFlight(User user, Flight flight)
        {
            var userFlight = context.UserFlight.Include(uf => uf.Flight).SingleOrDefault(uf => uf.FlightId == flight.Id && uf.UserId == user.Id);

            return userFlight;
        }

        public UserFlight GetUserFlightById(User user, int id)
        {
            var userFlight = context.UserFlight.Include(uf => uf.Flight).SingleOrDefault(uf => uf.FlightId == id && uf.UserId == user.Id);

            return userFlight;
        }

        public Flight UpdateFlight(int id, Flight flight)
        {
            var flightToUpdate = context.Flight.FirstOrDefault(flight => flight.Id == id);

            context.Entry(flightToUpdate).CurrentValues.SetValues(flight);

            var rows = context.SaveChanges();

            if (rows > 0)
            {
                return context.Flight.FirstOrDefault(flight => flight.Id == id);
            }

            return flight;
        }
    }
}