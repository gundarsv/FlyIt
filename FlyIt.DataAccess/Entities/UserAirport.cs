using FlyIt.DataAccess.Entities.Identity;

namespace FlyIt.DataAccess.Entities
{
    public class UserAirport
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int AirportId { get; set; }

        public virtual Airport Airport { get; set; }
    }
}