﻿using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly FlyItContext context;

        public AirportRepository(FlyItContext context)
        {
            this.context = context;
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

        public Task<List<Airport>> GetAirportsAsync()
        {
            return context.Airport.ToListAsync();
        }

        public Task<Airport> GetAirportByIdAsync(int id)
        {
            return context.Airport.SingleOrDefaultAsync(airport => airport.Id == id);
        }

        public async Task<UserAirport> AddUserAirportAsync(User user, Airport airport)
        {
            var userAirport = new UserAirport
            {
                User = user,
                Airport = airport
            };

            var entityEntry = await context.UserAirport.AddAsync(userAirport);

            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task<UserAirport> GetUserAirportByIdAsync(int userId, int airportId)
        {
            return await context.UserAirport.Include(ua => ua.Airport).SingleOrDefaultAsync(ua => ua.AirportId == airportId && ua.UserId == userId);
        }

        public async Task<UserAirport> RemoveUserAirportAsync(UserAirport userAirport)
        {
            var removedUserAirport = context.UserAirport.Remove(userAirport);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return removedUserAirport.Entity;
        }

        public async Task<Airport> RemoveAirportAsync(Airport airport)
        {
            var removedAirport = context.Airport.Remove(airport);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return removedAirport.Entity;
        }

        public async Task<Airport> GetAirportByIataAsync(string Iata)
        {
            return await context.Airport.SingleOrDefaultAsync(a => a.Iata == Iata);
        }

        public async Task<Airport> AddAirportAsync(Airport airport)
        {
            var entityEntry = await context.Airport.AddAsync(airport);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return entityEntry.Entity;
        }

        public async Task<List<UserAirport>> GetUserAirportsAsync(User user)
        {
            return await context.UserAirport.Include(ua => ua.Airport).Where(ua => ua.UserId == user.Id).ToListAsync();
        }
    }
}