using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IChatService
    {
        public Task<Result<ChatroomDTO>> GetChatroomById(ClaimsPrincipal claims, int chatroomId);

        public Task<Result<ChatroomMessageDTO>> AddMessage(ClaimsPrincipal claims, string message, int chatroomId);

        public Task<Result<ChatroomDTO>> AddUserToChatroom(ClaimsPrincipal claims, int chatroomId);

        public Task<Result<ChatroomDTO>> RemoveUserFromChatroom(ClaimsPrincipal claims, int chatroomId);
    }
}