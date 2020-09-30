using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.Domain.Models
{
    public class FlightDTO
    {
        public int Id { get; set; }

        public string FlightNo { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
