using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }

        public List<UserFlight> UserFlights { get; set; }

        public List<UserAirport> UserAirports { get; set; }
    }
}
