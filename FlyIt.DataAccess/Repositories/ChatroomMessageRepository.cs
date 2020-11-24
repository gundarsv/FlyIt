using FlyIt.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class ChatroomMessageRepository : IChatroomMessageRepository
    {
        private readonly FlyItContext flyItContext;

        public ChatroomMessageRepository(FlyItContext flyItContext)
        {
            this.flyItContext = flyItContext;
        }

        public async Task<ChatroomMessage> AddChatroomMessageAsync(ChatroomMessage chatroomMessage)
        {
            var enitityEntry = await flyItContext.ChatroomMessage.AddAsync(chatroomMessage);

            var result = await flyItContext.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return enitityEntry.Entity;
        }
    }
}