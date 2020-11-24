using FlyIt.DataAccess.Entities;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IChatroomMessageRepository
    {
        public Task<ChatroomMessage> AddChatroomMessageAsync(ChatroomMessage chatroomMessage);
    }
}