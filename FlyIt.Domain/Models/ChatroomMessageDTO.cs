using FlyIt.Domain.Models.Enums;
using System;

namespace FlyIt.Domain.Models
{
    public class ChatroomMessageDTO
    {
        public string UserName { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public MessageType MessageType { get; set; }
    }
}