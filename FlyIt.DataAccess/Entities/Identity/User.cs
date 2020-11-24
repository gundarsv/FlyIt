using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }

        public virtual ICollection<UserFlight> UserFlights { get; set; }

        public virtual ICollection<UserAirport> UserAirports { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }

        public virtual ICollection<UserChatroom> UserChatrooms { get; set; }

        public virtual ICollection<ChatroomMessage> ChatroomMessages { get; set; }
    }
}