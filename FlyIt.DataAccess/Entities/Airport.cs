using FlyIt.DataAccess.Entities.Identity;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities
{
    public class Airport
    {
        public int Id { get; set; }

        public string Iata { get; set; }

        public string Name { get; set; }

        public string MapUrl { get; set; }

        public string MapName { get; set; }

        public string RentingCompanyUrl { get; set; }

        public string RentingCompanyName { get; set; }

        public string RentingCompanyPhoneNo { get; set; }

        public string TaxiPhoneNo { get; set; }

        public string EmergencyPhoneNo { get; set; }

        public string Icao { get; set; }

        public virtual ICollection<UserAirport> UserAirports { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}