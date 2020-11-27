using FlyIt.Api.Hubs;
using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using FlyIt.Domain.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FlyIt.Api.Test.Hubs
{
    public class ChatHubTest
    {
        private readonly Mock<IUserService> userService;
        private readonly Mock<IChatService> chatService;
        private readonly Mock<ILogger<ChatHub>> logger;
        private readonly ChatHub chatHub;

        private Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
        private Mock<IGroupManager> mockGroups = new Mock<IGroupManager>();
        private Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
        private Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();

        public ChatHubTest()
        {
            userService = new Mock<IUserService>();
            chatService = new Mock<IChatService>();
            logger = new Mock<ILogger<ChatHub>>();

            mockClients.Setup(client => client.All).Returns(mockClientProxy.Object);
            mockClients.Setup(client => client.Caller).Returns(mockClientProxy.Object);
            mockGroups.Setup(group => group.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken())).Returns(Task.FromResult(true));
            mockGroups.Setup(group => group.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken())).Returns(Task.FromResult(true));
            mockContext.Setup(context => context.ConnectionId).Returns(It.IsAny<string>());
            mockContext.Setup(context => context.User).Returns(It.IsAny<ClaimsPrincipal>());

            chatHub = new ChatHub(chatService.Object, userService.Object, logger.Object)
            {
                Clients = mockClients.Object,
                Groups = mockGroups.Object,
                Context = mockContext.Object
            };
        }

        [TestClass]
        public class StartChat : ChatHubTest
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { new UnexpectedResult<UserDTO>() };
                    yield return new object[] { new InvalidResult<UserDTO>("invalid") };
                    yield return new object[] { new NotFoundResult<UserDTO>("notFound") };
                }
            }

            [DataTestMethod]
            [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
            public async Task CallsCallerIfGetUserReturnsNotSuccess(Result<UserDTO> getUserResult)
            {
                userService.Setup(us => us.GetUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(getUserResult);

                await chatHub.StartChat(It.IsAny<int>());

                mockClients.Verify(clients => clients.Caller, Times.Once);
                mockGroups.Verify(groups => groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [TestMethod]
            public async Task SendsToGroup()
            {
                userService.Setup(us => us.GetUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new SuccessResult<UserDTO>(new UserDTO()
                {
                    Email = "test@test.com"
                }));

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

                await chatHub.StartChat(It.IsAny<int>());

                mockClients.Verify(clients => clients.Caller, Times.Never);
                mockGroups.Verify(groups => groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
                mockClients.Verify(groups => groups.Group(It.IsAny<string>()), Times.Once);
            }

            [TestMethod]
            public async Task ThrowsHubExceptionIfThrowsException()
            {
                userService.Setup(us => us.GetUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new SuccessResult<UserDTO>(new UserDTO()
                {
                    Email = "test@test.com"
                }));

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Throws(new Exception());

                await Assert.ThrowsExceptionAsync<HubException>(async () =>
                {
                    await chatHub.StartChat(It.IsAny<int>());
                });

                mockClients.Verify(clients => clients.Caller, Times.Never);
                mockGroups.Verify(groups => groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
                mockClients.Verify(groups => groups.Group(It.IsAny<string>()), Times.Once);
            }
        }

        [TestClass]
        public class EndChat : ChatHubTest
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { new UnexpectedResult<UserDTO>() };
                    yield return new object[] { new InvalidResult<UserDTO>("invalid") };
                    yield return new object[] { new NotFoundResult<UserDTO>("notFound") };
                }
            }

            [DataTestMethod]
            [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
            public async Task CallsCallerIfGetUserReturnsNotSuccess(Result<UserDTO> getUserResult)
            {
                userService.Setup(us => us.GetUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(getUserResult);

                await chatHub.EndChat(It.IsAny<int>());

                mockClients.Verify(clients => clients.Caller, Times.Once);
                mockGroups.Verify(groups => groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [TestMethod]
            public async Task SendsToGroup()
            {
                userService.Setup(us => us.GetUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new SuccessResult<UserDTO>(new UserDTO()
                {
                    Email = "test@test.com"
                }));

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

                await chatHub.EndChat(It.IsAny<int>());

                mockClients.Verify(clients => clients.Caller, Times.Never);
                mockGroups.Verify(groups => groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
                mockClients.Verify(groups => groups.Group(It.IsAny<string>()), Times.Once);
            }

            [TestMethod]
            public async Task ThrowsHubExceptionIfThrowsException()
            {
                userService.Setup(us => us.GetUser(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new SuccessResult<UserDTO>(new UserDTO()
                {
                    Email = "test@test.com"
                }));

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Throws(new Exception());

                await Assert.ThrowsExceptionAsync<HubException>(async () =>
                {
                    await chatHub.EndChat(It.IsAny<int>());
                });

                mockClients.Verify(clients => clients.Caller, Times.Never);
                mockGroups.Verify(groups => groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
                mockClients.Verify(groups => groups.Group(It.IsAny<string>()), Times.Once);
            }
        }

        [TestClass]
        public class SendMessage : ChatHubTest
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { new UnexpectedResult<ChatroomMessageDTO>() };
                    yield return new object[] { new InvalidResult<ChatroomMessageDTO>("invalid") };
                    yield return new object[] { new NotFoundResult<ChatroomMessageDTO>("notFound") };
                }
            }

            [DataTestMethod]
            [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
            public async Task CallsCallerIfAddMessageReturnsNotSuccess(Result<ChatroomMessageDTO> addMessageResult)
            {
                chatService.Setup(cs => cs.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(addMessageResult);

                await chatHub.SendMessage(It.IsAny<int>(), It.IsAny<string>());

                mockClients.Verify(clients => clients.Caller, Times.Once);
                mockClients.Verify(clients => clients.Group(It.IsAny<string>()), Times.Never);
            }

            [TestMethod]
            public async Task SendsToGroup()
            {
                chatService.Setup(cs => cs.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new SuccessResult<ChatroomMessageDTO>(new ChatroomMessageDTO()));

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

                await chatHub.SendMessage(It.IsAny<int>(), It.IsAny<string>());

                mockClients.Verify(clients => clients.Caller, Times.Never);
                mockClients.Verify(groups => groups.Group(It.IsAny<string>()), Times.Once);
            }

            [TestMethod]
            public async Task ThrowsHubExceptionIfThrowsException()
            {
                chatService.Setup(cs => cs.AddMessage(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new SuccessResult<ChatroomMessageDTO>(new ChatroomMessageDTO()));

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Throws(new Exception());

                await Assert.ThrowsExceptionAsync<HubException>(async () =>
                {
                    await chatHub.SendMessage(It.IsAny<int>(), It.IsAny<string>());
                });

                mockClients.Verify(clients => clients.Caller, Times.Never);
                mockClients.Verify(groups => groups.Group(It.IsAny<string>()), Times.Once);
            }
        }
    }
}