using System;

namespace FlyIt.Domain.Models
{
    public class FlightSearchDTO
    {
        public string FlightNo { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
