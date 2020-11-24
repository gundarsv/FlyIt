using FlyIt.DataAccess.Entities.Identity;
using System;

namespace FlyIt.DataAccess.Entities
{
    public class ChatroomMessage
    {
        public int Id { get; set; }

        public int ChatroomId { get; set; }

        public virtual Chatroom Chatroom { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime DateTime { get; set; }

        public string Message { get; set; }
    }
}