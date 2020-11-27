using FlyIt.Domain.Models;
using FlyIt.Domain.Models.Enums;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FlyIt.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;
        private readonly ILogger<ChatHub> logger;

        public ChatHub(IChatService chatService, IUserService userService, ILogger<ChatHub> logger)
        {
            this.chatService = chatService;
            this.userService = userService;
            this.logger = logger;
        }

        public async Task StartChat(int chatroomId)
        {
            try
            {
                var user = await userService.GetUser(Context.User);

                if (!user.ResultType.Equals(ResultType.Ok))
                {
                    await Clients.Caller.SendAsync("receiveMessage", "User not found");
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId.ToString());

                var message = new ChatroomMessageDTO()
                {
                    Message = $"{user.Data.Email} is now in chat room",
                    MessageType = MessageType.Information,
                    DateTime = DateTime.Now,
                    UserName = user.Data.Email
                };

                await Clients.Group(chatroomId.ToString()).SendAsync("receiveChatMessage", message);
            }
            catch (Exception ex)
            {
                logger.LogError($"StartChat: {chatroomId}", ex);
                throw new HubException("Something went wrong");
            }
        }

        public async Task EndChat(int chatroomId)
        {
            try
            {
                var user = await userService.GetUser(Context.User);

                if (!user.ResultType.Equals(ResultType.Ok))
                {
                    await Clients.Caller.SendAsync("receiveMessage", "User not found");
                    return;
                }

                var message = new ChatroomMessageDTO()
                {
                    Message = $"{user.Data.Email} has now left the chat room",
                    MessageType = MessageType.Information,
                    DateTime = DateTime.Now,
                    UserName = user.Data.Email
                };

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId.ToString());

                await Clients.Group(chatroomId.ToString()).SendAsync("receiveChatMessage", message);
            }
            catch (Exception ex)
            {
                logger.LogError($"EndChat: {chatroomId}", ex);
                throw new HubException("Something went wrong");
            }
        }

        public async Task SendMessage(int chatroomId, string message)
        {
            try
            {
                var chatroomMessage = await chatService.AddMessage(Context.User, message, chatroomId);

                if (!chatroomMessage.ResultType.Equals(ResultType.Ok))
                {
                    await Clients.Caller.SendAsync("receiveMessage", "Chatroom message was not sent");
                    return;
                }

                chatroomMessage.Data.MessageType = MessageType.Message;

                await Clients.Group(chatroomId.ToString()).SendAsync("receiveChatMessage", chatroomMessage.Data);
            }
            catch (Exception ex)
            {
                logger.LogError($"SendMessage: {chatroomId} with message {message}", ex);
                throw new HubException("Something went wrong");
            }
        }
    }
}