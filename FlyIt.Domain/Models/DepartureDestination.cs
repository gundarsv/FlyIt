using System;

namespace FlyIt.Domain.Models
{
    public class DepartureDestination
    {
        public string Gate { get; set; }

        public int? Delay { get; set; }

        public string Terminal { get; set; }

        public string AirportName { get; set; }

        public DateTimeOffset? Scheduled { get; set; }

        public DateTimeOffset? Estimated { get; set; }

        public DateTimeOffset? Actual { get; set; }

        public string Iata { get; set; }

        public string Icao { get; set; }
    }
}