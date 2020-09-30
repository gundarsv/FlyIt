using FlyIt.DataAccess.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.DataAccess.Entities
{
    public class UserFlight
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public Flight Flight { get; set; }

        public int FlightId { get; set; }
    }
}
