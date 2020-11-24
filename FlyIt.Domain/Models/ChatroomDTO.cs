using System;
using System.Collections.Generic;

namespace FlyIt.Domain.Models
{
    public class ChatroomDTO
    {
        public int Id { get; set; }

        public string FlightNo { get; set; }

        public DateTimeOffset Date { get; set; }

        public bool HasUserJoined { get; set; }

        public List<ChatroomMessageDTO> ChatroomMessages { get; set; }
    }
}