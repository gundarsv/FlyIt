using FlyIt.DataAccess.Entities;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IUserChatroomRepository
    {
        public Task<UserChatroom> AddUserChatroomAsync(UserChatroom userChatroom);

        public Task<UserChatroom> RemoveUserChatroomAsync(UserChatroom userChatroom);

        public Task<UserChatroom> GetUserChatroomByIdAsync(int chatroomId, int userId);
    }
}