using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Mappings;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class ChatServiceTest
    {
        private readonly IMapper mapper;
        private readonly Mock<IUserChatroomRepository> userChatroomRepository;
        private readonly Mock<IChatroomRepository> chatroomRepository;
        private readonly Mock<IChatroomMessageRepository> chatroomMessageRepository;
        private readonly Mock<UserManager<User>> userManager;
        private readonly ChatService chatService;

        private readonly User user;
        private readonly Chatroom chatroom;
        private readonly UserChatroom userChatroom;
        private readonly ChatroomMessage chatroomMessage;

        public ChatServiceTest()
        {
            var userStore = Mock.Of<IUserStore<User>>();
            userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            userChatroomRepository = new Mock<IUserChatroomRepository>();
            chatroomRepository = new Mock<IChatroomRepository>();
            chatroomMessageRepository = new Mock<IChatroomMessageRepository>();

            var chatroomMappingProfile = new ChatroomMapping();
            var chatroomMessageMappingProfile = new ChatroomMessageMapping();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { chatroomMappingProfile, chatroomMessageMappingProfile }));
            mapper = new Mapper(configuration);

            chatService = new ChatService(userManager.Object, mapper, chatroomRepository.Object, userChatroomRepository.Object, chatroomMessageRepository.Object);

            user = new User();
            chatroom = new Chatroom();
            userChatroom = new UserChatroom()
            {
                User = user,
                UserId = user.Id,
                Chatroom = chatroom,
                ChatroomId = chatroom.Id
            };
            chatroomMessage = new ChatroomMessage();
        }

        [TestClass]
        public class GetChatroomById : ChatServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await chatService.GetChatroomById(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfChatroomNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync((Chatroom)null);

                var result = await chatService.GetChatroomById(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessUserJoinedFalse()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserChatroom)null);

                var result = await chatService.GetChatroomById(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.IsFalse(result.Data.HasUserJoined);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessUserJoinedTrue()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);

                var result = await chatService.GetChatroomById(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.IsTrue(result.Data.HasUserJoined);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception());

                var result = await chatService.GetChatroomById(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class AddMessage : ChatServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await chatService.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfChatroomNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync((Chatroom)null);

                var result = await chatService.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserChatroomNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserChatroom)null);

                var result = await chatService.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);
                chatroomMessageRepository.Setup(crmr => crmr.AddChatroomMessageAsync(It.IsAny<ChatroomMessage>())).ReturnsAsync((ChatroomMessage)null);

                var result = await chatService.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                chatroomMessageRepository.Verify(crmr => crmr.AddChatroomMessageAsync(It.IsAny<ChatroomMessage>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);
                chatroomMessageRepository.Setup(crmr => crmr.AddChatroomMessageAsync(It.IsAny<ChatroomMessage>())).ReturnsAsync(chatroomMessage);

                var result = await chatService.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                chatroomMessageRepository.Verify(crmr => crmr.AddChatroomMessageAsync(It.IsAny<ChatroomMessage>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);
                chatroomMessageRepository.Setup(crmr => crmr.AddChatroomMessageAsync(It.IsAny<ChatroomMessage>())).ThrowsAsync(new Exception());

                var result = await chatService.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                chatroomMessageRepository.Verify(crmr => crmr.AddChatroomMessageAsync(It.IsAny<ChatroomMessage>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class AddUserToChatroom : ChatServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await chatService.AddUserToChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfChatroomNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync((Chatroom)null);

                var result = await chatService.AddUserToChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserChatroomFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);

                var result = await chatService.AddUserToChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserChatroomNotAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserChatroom)null);
                userChatroomRepository.Setup(uccr => uccr.AddUserChatroomAsync(It.IsAny<UserChatroom>())).ReturnsAsync((UserChatroom)null);

                var result = await chatService.AddUserToChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.AddUserChatroomAsync(It.IsAny<UserChatroom>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUserChatroomAdded()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserChatroom)null);
                userChatroomRepository.Setup(uccr => uccr.AddUserChatroomAsync(It.IsAny<UserChatroom>())).ReturnsAsync(userChatroom);

                var result = await chatService.AddUserToChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.AddUserChatroomAsync(It.IsAny<UserChatroom>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserChatroom)null);
                userChatroomRepository.Setup(uccr => uccr.AddUserChatroomAsync(It.IsAny<UserChatroom>())).ThrowsAsync(new Exception());

                var result = await chatService.AddUserToChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.AddUserChatroomAsync(It.IsAny<UserChatroom>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class RemoveUserFromChatroom : ChatServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

                var result = await chatService.RemoveUserFromChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfChatroomNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync((Chatroom)null);

                var result = await chatService.RemoveUserFromChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserChatroomNotFound()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserChatroom)null);

                var result = await chatService.RemoveUserFromChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfUserChatroomNotRemoved()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);
                userChatroomRepository.Setup(uccr => uccr.RemoveUserChatroomAsync(It.IsAny<UserChatroom>())).ReturnsAsync((UserChatroom)null);

                var result = await chatService.RemoveUserFromChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.RemoveUserChatroomAsync(It.IsAny<UserChatroom>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfUserChatroomRemoved()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);
                userChatroomRepository.Setup(uccr => uccr.RemoveUserChatroomAsync(It.IsAny<UserChatroom>())).ReturnsAsync(userChatroom);

                var result = await chatService.RemoveUserFromChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.RemoveUserChatroomAsync(It.IsAny<UserChatroom>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                chatroomRepository.Setup(crr => crr.GetChatroomByIdAsync(It.IsAny<int>())).ReturnsAsync(chatroom);
                userChatroomRepository.Setup(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(userChatroom);
                userChatroomRepository.Setup(uccr => uccr.RemoveUserChatroomAsync(It.IsAny<UserChatroom>())).ThrowsAsync(new Exception());

                var result = await chatService.RemoveUserFromChatroom(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>());

                userManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
                chatroomRepository.Verify(crr => crr.GetChatroomByIdAsync(It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.GetUserChatroomByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                userChatroomRepository.Verify(uccr => uccr.RemoveUserChatroomAsync(It.IsAny<UserChatroom>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }
    }
}