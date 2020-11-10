using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.Domain.Models
{
    internal class WeatherDTO
    {
        public double Temperature { get; set; }

        public string WeatherStatus { get; set; }
    }
}