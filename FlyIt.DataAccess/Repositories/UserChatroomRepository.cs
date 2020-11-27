using FlyIt.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class UserChatroomRepository : IUserChatroomRepository
    {
        private readonly FlyItContext context;

        public UserChatroomRepository(FlyItContext context)
        {
            this.context = context;
        }

        public async Task<UserChatroom> AddUserChatroomAsync(UserChatroom userChatroom)
        {
            if (userChatroom is null)
            {
                return null;
            }

            var entityEntry = await context.UserChatroom.AddAsync(userChatroom);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return entityEntry.Entity;
        }

        public async Task<UserChatroom> GetUserChatroomByIdAsync(int chatroomId, int userId)
        {
            return await context.UserChatroom.SingleOrDefaultAsync(cr => cr.ChatroomId == chatroomId && cr.UserId == userId);
        }

        public async Task<UserChatroom> RemoveUserChatroomAsync(UserChatroom userChatroom)
        {
            if (userChatroom is null)
            {
                return null;
            }

            var removedUserChatroom = context.UserChatroom.Remove(userChatroom);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return removedUserChatroom.Entity;
        }
    }
}