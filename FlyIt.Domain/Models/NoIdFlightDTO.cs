using System;

namespace FlyIt.Domain.Models
{
    public class NoIdFlightDTO
    {
        public string FlightNo { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
