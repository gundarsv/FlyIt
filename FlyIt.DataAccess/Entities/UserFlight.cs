using FlyIt.DataAccess.Entities.Identity;

namespace FlyIt.DataAccess.Entities
{
    public class UserFlight
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Flight Flight { get; set; }

        public int FlightId { get; set; }
    }
}