using FlyIt.DataAccess.Entities;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IUserChatroomRepository
    {
        public Task<UserChatroom> AddUserToChatroomAsync(UserChatroom userChatroom);

        public Task<UserChatroom> RemoveUserFromChatroomAsync(UserChatroom userChatroom);

        public Task<UserChatroom> GetUserChatroomById(int chatroomId, int userId);
    }
}