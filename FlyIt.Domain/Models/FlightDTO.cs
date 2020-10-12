using System;

namespace FlyIt.Domain.Models
{
    public class FlightDTO
    {
        public int Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public string FlightNo { get; set; }

        public string Status { get; set; }

        public DepartureDestination Departure { get; set; }

        public DepartureDestination Destination { get; set; }
    }
}