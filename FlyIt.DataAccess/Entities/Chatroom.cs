using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities
{
    public class Chatroom
    {
        public int Id { get; set; }

        public int FlightId { get; set; }

        public virtual Flight Flight { get; set; }

        public virtual ICollection<UserChatroom> UserChatrooms { get; set; }

        public virtual ICollection<ChatroomMessage> ChatroomMessages { get; set; }
    }
}