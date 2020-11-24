using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FlyIt.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatHub(IChatService chatService, IUserService userService)
        {
            this.chatService = chatService;
            this.userService = userService;
        }

        public async Task StartChat(string chatroomId)
        {
            var user = await userService.GetUser(Context.User);

            if (!user.ResultType.Equals(ResultType.Ok))
            {
                await Clients.Caller.SendAsync("receiveMessage", "User not found");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);

            await Clients.Groups(chatroomId).SendAsync("receiveChatMessage", $"{user.Data.Email} joined chat room");
        }

        public async Task EndChat(string chatroomId)
        {
            var user = await userService.GetUser(Context.User);

            if (!user.ResultType.Equals(ResultType.Ok))
            {
                await Clients.Caller.SendAsync("receiveMessage", "User not found");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId);

            await Clients.Groups(chatroomId).SendAsync("receiveChatMessage", $"{user.Data.Email} has left chat room");
        }

        public async Task SendMessage(int chatroomId, string message)
        {
            var chatroomMessage = await chatService.AddMessage(Context.User, message, chatroomId);

            if (!chatroomMessage.ResultType.Equals(ResultType.Ok))
            {
                await Clients.Caller.SendAsync("receiveMessage", "Chatroom message was not sent");
            }

            await Clients.Groups(chatroomId.ToString()).SendAsync("receiveChatMessage", chatroomMessage);
        }
    }
}