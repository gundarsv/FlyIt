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

        [Required]
        public string RentingCompanyUrl { get; set; }

        [Required]
        public string RentingCompanyName { get; set; }

        [Required]
        public string RentingCompanyPhoneNo { get; set; }

        [Required]
        public string TaxiPhoneNo { get; set; }

        [Required]
        public string EmergencyPhoneNo { get; set; }

        [Required]
        public string Icao { get; set; }
    }
}