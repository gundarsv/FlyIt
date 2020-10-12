using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyIt.DataAccess.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly FlyItContext context;

        public AirportRepository(FlyItContext context)
        {
            this.context = context;
        }

        public Airport AddAirport(Airport airport)
        {
            var entityEntry = context.Airport.Add(airport);

            context.SaveChanges();

            return entityEntry.Entity;
        }

        public Airport GetAirportById(int id)
        {
            return context.Airport.SingleOrDefault(airport => airport.Id == id);
        }

        public UserAirport AddUserAirport(User user, Airport airport)
        {
            var userAirport = new UserAirport
            {
                User = user,
                Airport = airport
            };

            var entityEntry = context.UserAirport.Add(userAirport);

            context.SaveChanges();

            return entityEntry.Entity;
        }

        public List<UserAirport> GetUserAirports(User user)
        {
            return context.UserAirport.Include(ua => ua.Airport).Where(ua => ua.UserId == user.Id).ToList();
        }

        public Airport GetAirportByIata(string iata)
        {
            return context.Airport.SingleOrDefault(a => a.Iata == iata);
        }

        public UserAirport RemoveUserAirport(UserAirport userAirport)
        {
            var removedUserAirport = context.UserAirport.Remove(userAirport);

            context.SaveChanges();

            return removedUserAirport.Entity;
        }

        public UserAirport GetUserAirport(User user, Airport airport)
        {
            var userAirport = context.UserAirport.Include(ua => ua.Airport).SingleOrDefault(ua => ua.AirportId == airport.Id && ua.UserId == user.Id);

            return userAirport;
        }

        public UserAirport GetUserAirportById(User user, int id)
        {
            var userAirport = context.UserAirport.Include(ua => ua.Airport).SingleOrDefault(ua => ua.AirportId == id && ua.UserId == user.Id);

            return userAirport;
        }

        public Airport UpdateAirport(int id, Airport airport)
        {
            var airportToUpdate = context.Airport.FirstOrDefault(airport => airport.Id == id);

            context.Entry(airportToUpdate).CurrentValues.SetValues(airport);

            var rows = context.SaveChanges();

            if (rows > 0)
            {
                return context.Airport.FirstOrDefault(airport => airport.Id == id);
            }

            return airport;
        }
    }
}
