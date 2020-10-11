using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities
{
   public class Airport
    {
        public int Id { get; set; }

        public string Iata { get; set; }

        public List<UserAirport> UserAirports { get; set; }
    }
}
