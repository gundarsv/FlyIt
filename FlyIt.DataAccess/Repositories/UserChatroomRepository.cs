using FlyIt.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<UserChatroom> AddUserToChatroomAsync(UserChatroom userChatroom)
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

        public async Task<UserChatroom> GetUserChatroomById(int chatroomId, int userId)
        {
            return await context.UserChatroom.SingleOrDefaultAsync(cr => cr.ChatroomId == chatroomId && cr.UserId == userId);
        }

        public async Task<UserChatroom> RemoveUserFromChatroomAsync(UserChatroom userChatroom)
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