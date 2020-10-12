using FlyIt.DataAccess.Entities.Identity;

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