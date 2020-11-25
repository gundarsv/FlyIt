﻿using FlyIt.Domain.Models;
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

        public async Task StartChat(string chatroomId)
        {
            try
            {
                var user = await userService.GetUser(Context.User);

                if (!user.ResultType.Equals(ResultType.Ok))
                {
                    await Clients.Caller.SendAsync("receiveMessage", "User not found");
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, chatroomId);

                var message = new ChatroomMessageDTO()
                {
                    Message = $"{user.Data.Email} is now in chat room",
                    MessageType = MessageType.Information,
                    DateTime = DateTime.Now,
                    UserName = user.Data.Email
                };

                await Clients.Groups(chatroomId).SendAsync("receiveChatMessage", message);
            }
            catch (Exception ex)
            {
                logger.LogError($"StartChat: {chatroomId}", ex);
                throw new HubException("Something went wrong");
            }
        }

        public async Task EndChat(string chatroomId)
        {
            try
            {
                var user = await userService.GetUser(Context.User);

                if (!user.ResultType.Equals(ResultType.Ok))
                {
                    await Clients.Caller.SendAsync("receiveMessage", "User not found");
                }

                var message = new ChatroomMessageDTO()
                {
                    Message = $"{user.Data.Email} has now left the chat room",
                    MessageType = MessageType.Information,
                    DateTime = DateTime.Now,
                    UserName = user.Data.Email
                };

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatroomId);

                await Clients.Groups(chatroomId).SendAsync("receiveChatMessage", message);
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
                }

                chatroomMessage.Data.MessageType = MessageType.Message;

                await Clients.Groups(chatroomId.ToString()).SendAsync("receiveChatMessage", chatroomMessage.Data);
            }
            catch (Exception ex)
            {
                logger.LogError($"SendMessage: {chatroomId} with message {message}", ex);
                throw new HubException("Something went wrong");
            }
        }
    }
}