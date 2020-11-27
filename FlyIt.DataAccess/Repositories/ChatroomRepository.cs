using FlyIt.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class ChatroomRepository : IChatroomRepository
    {
        private readonly FlyItContext flyItContext;

        public ChatroomRepository(FlyItContext flyItContext)
        {
            this.flyItContext = flyItContext;
        }

        public async Task<Chatroom> GetChatroomByIdAsync(int id)
        {
            return await flyItContext.Chatroom.Include(cr => cr.ChatroomMessages).Include(cr => cr.UserChatrooms).SingleOrDefaultAsync(cr => cr.Id == id);
        }
    }
}