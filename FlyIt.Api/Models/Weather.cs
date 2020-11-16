using System.ComponentModel.DataAnnotations;

namespace FlyIt.Api.Models
{
    public class Weather
    {
        [Required]
        public double Temperature { get; set; }

        [Required]
        public string WeatherStatus { get; set; }
    }
}