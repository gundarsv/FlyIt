using System.ComponentModel.DataAnnotations;

namespace FlyIt.Api.Models
{
    public class Airport
    {
        [Required]
        public string Iata { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string MapUrl { get; set; }

        [Required]
        public string MapName { get; set; }
    }
}