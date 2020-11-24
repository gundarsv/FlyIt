using FlyIt.DataAccess.Entities.Identity;
using System.Collections.Generic;

namespace FlyIt.DataAccess.Entities
{
    public class UserChatroom
    {
        public int ChatroomId { get; set; }

        public virtual Chatroom ChatRoom { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}