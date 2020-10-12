using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;


namespace FlyIt.DataAccess.Entities
{
    public class UserAirport
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public Airport Airport { get; set; }

        public int AirportId { get; set; }
    }
}
