using FlyIt.Api.Extensions;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlyIt.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet("{chatroomId}")]
        public async Task<IActionResult> GetChatroom(int chatroomId)
        {
            var result = await chatService.GetChatroomById(User, chatroomId);

            return this.FromResult(result);
        }

        [HttpPost("{chatroomId}")]
        public async Task<IActionResult> JoinChatroom(int chatroomId)
        {
            var result = await chatService.AddUserToChatroom(User, chatroomId);

            return this.FromResult(result);
        }

        [HttpDelete("{chatroomId}")]
        public async Task<IActionResult> LeaveChatroom(int chatroomId)
        {
            var result = await chatService.RemoveUserFromChatroom(User, chatroomId);

            return this.FromResult(result);
        }
    }
}