using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Repositories
{
    public interface IFlightRepository
    {
        public Flight AddFlight(Flight flight);

        public Flight GetFlightById(int id);

        public UserFlight AddUserFlight(User user, Flight flight);

        public Flight UpdateFlight(int id, Flight flight);

        public List<UserFlight> GetUserFlights(User user);

        public Flight GetFlightByDateAndFlightNumber(DateTimeOffset date, string flightNo);

        public UserFlight GetUserFlight(User user, Flight flight);

        public UserFlight GetUserFlightById(User user, int id);

        public UserFlight RemoveUserFlight(UserFlight userFlight);
    }
}