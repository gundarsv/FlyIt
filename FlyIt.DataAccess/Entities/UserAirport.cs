using FlyIt.DataAccess.Entities.Identity;

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