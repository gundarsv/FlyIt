using System;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities
{
    public class Flight
    {
        public int Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public string FlightNo { get; set; }

        public string Status { get; set; }

        public string DepartureIata { get; set; }

        public string DepartureGate { get; set; }

        public int? DepartureDelay { get; set; }

        public string DepartureTerminal { get; set; }

        public string DepartureAirportName { get; set; }

        public DateTimeOffset? DepartureScheduled { get; set; }

        public DateTimeOffset? DepartureEstimated { get; set; }

        public DateTimeOffset? DepartureActual { get; set; }

        public string DestinationIata { get; set; }

        public string DestinationGate { get; set; }

        public int? DestinationDelay { get; set; }

        public string DestinationTerminal { get; set; }

        public string DestinationAirportName { get; set; }

        public DateTimeOffset? DestinationScheduled { get; set; }

        public DateTimeOffset? DestinationEstimated { get; set; }

        public DateTimeOffset? DestinationActual { get; set; }

        public virtual ICollection<UserFlight> UserFlights { get; set; }
    }
}