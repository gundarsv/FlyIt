using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class ChatService : IChatService
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IUserChatroomRepository userChatroomRepository;
        private readonly IChatroomRepository chatroomRepository;
        private readonly IChatroomMessageRepository chatroomMessageRepository;

        public ChatService(UserManager<User> userManager, IMapper mapper, IChatroomRepository chatroomRepository, IUserChatroomRepository userChatroomRepository, IChatroomMessageRepository chatroomMessageRepository)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.userChatroomRepository = userChatroomRepository;
            this.chatroomRepository = chatroomRepository;
            this.chatroomMessageRepository = chatroomMessageRepository;
        }

        public async Task<Result<ChatroomDTO>> GetChatroomById(ClaimsPrincipal claims, int chatroomId)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<ChatroomDTO>("User not found");
                }

                var chatroom = await chatroomRepository.GetChatroomByIdAsync(chatroomId);

                if (chatroom is null)
                {
                    return new NotFoundResult<ChatroomDTO>("Chatroom not found");
                }

                var result = mapper.Map<Chatroom, ChatroomDTO>(chatroom);

                var userChatroom = await userChatroomRepository.GetUserChatroomByIdAsync(chatroom.Id, user.Id);

                result.HasUserJoined = userChatroom is null ? false : true;

                return new SuccessResult<ChatroomDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<ChatroomDTO>(ex.Message);
            }
        }

        public async Task<Result<ChatroomMessageDTO>> AddMessage(ClaimsPrincipal claims, string message, int chatroomId)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<ChatroomMessageDTO>("User not found");
                }

                var chatroom = await chatroomRepository.GetChatroomByIdAsync(chatroomId);

                if (chatroom is null)
                {
                    return new NotFoundResult<ChatroomMessageDTO>("Chatroom not found");
                }

                var userChatroom = await userChatroomRepository.GetUserChatroomByIdAsync(chatroom.Id, user.Id);

                if (userChatroom is null)
                {
                    return new InvalidResult<ChatroomMessageDTO>($"User is not part of chatroom with id: {chatroomId}");
                }

                var addedChatroomMessage = await chatroomMessageRepository.AddChatroomMessageAsync(new ChatroomMessage()
                {
                    ChatroomId = chatroom.Id,
                    Chatroom = chatroom,
                    UserId = user.Id,
                    User = user,
                    Message = message,
                    DateTime = DateTime.Now
                });

                if (addedChatroomMessage is null)
                {
                    return new InvalidResult<ChatroomMessageDTO>("Chatroom message could not be added");
                }

                var result = mapper.Map<ChatroomMessage, ChatroomMessageDTO>(addedChatroomMessage);

                return new SuccessResult<ChatroomMessageDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<ChatroomMessageDTO>(ex.Message);
            }
        }

        public async Task<Result<ChatroomDTO>> AddUserToChatroom(ClaimsPrincipal claims, int chatroomId)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<ChatroomDTO>("User not found");
                }

                var chatroom = await chatroomRepository.GetChatroomByIdAsync(chatroomId);

                if (chatroom is null)
                {
                    return new NotFoundResult<ChatroomDTO>("Chatroom not found");
                }

                var userChatroom = await userChatroomRepository.GetUserChatroomByIdAsync(chatroomId, user.Id);

                if (userChatroom is not null)
                {
                    return new InvalidResult<ChatroomDTO>("User has already joined the chatroom");
                }

                var addedUserToChatroom = await userChatroomRepository.AddUserChatroomAsync(new UserChatroom()
                {
                    User = user,
                    UserId = user.Id,
                    Chatroom = chatroom,
                    ChatroomId = chatroom.Id,
                });

                if (addedUserToChatroom is null)
                {
                    return new InvalidResult<ChatroomDTO>("User could not be added to chatroom");
                }

                var result = mapper.Map<UserChatroom, ChatroomDTO>(addedUserToChatroom);

                result.HasUserJoined = true;

                return new SuccessResult<ChatroomDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<ChatroomDTO>(ex.Message);
            }
        }

        public async Task<Result<ChatroomDTO>> RemoveUserFromChatroom(ClaimsPrincipal claims, int chatroomId)
        {
            try
            {
                var user = await userManager.GetUserAsync(claims);

                if (user is null)
                {
                    return new NotFoundResult<ChatroomDTO>("User not found");
                }

                var chatroom = await chatroomRepository.GetChatroomByIdAsync(chatroomId);

                if (chatroom is null)
                {
                    return new NotFoundResult<ChatroomDTO>("Chatroom not found");
                }

                var userChatroom = await userChatroomRepository.GetUserChatroomByIdAsync(chatroomId, user.Id);

                if (userChatroom is null)
                {
                    return new InvalidResult<ChatroomDTO>($"User is not part of chatroom with id: {chatroomId}");
                }

                var removedUserChatroom = await userChatroomRepository.RemoveUserChatroomAsync(userChatroom);

                if (removedUserChatroom is null)
                {
                    return new InvalidResult<ChatroomDTO>("Chatroom could not be removed");
                }

                var result = mapper.Map<UserChatroom, ChatroomDTO>(removedUserChatroom);

                result.HasUserJoined = false;

                return new SuccessResult<ChatroomDTO>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<ChatroomDTO>(ex.Message);
            }
        }
    }
}