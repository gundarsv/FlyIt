using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.Domain.Models
{
    public class WeatherDTO
    {
        public long Temperature { get; set; }

        public long? RealFeel { get; set; }

        public long Humidity { get; set; }

        public long WindSpeed { get; set; }

        public string City { get; set; }
    }
}