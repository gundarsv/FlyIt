using AutoMapper;
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
using System.Threading.Tasks;

namespace FlyIt.Domain.Test.Services
{
    public class RoleServiceTest
    {
        private readonly Mock<RoleManager<Role>> roleManager;
        private readonly Mock<UserManager<User>> userManager;
        private readonly Mock<IRoleRepository> repository;
        private readonly IMapper mapper;
        private readonly RoleService roleService;

        public RoleServiceTest()
        {
            var roleStore = Mock.Of<IRoleStore<Role>>();
            roleManager = new Mock<RoleManager<Role>>(roleStore, null, null, null, null);

            var userStore = Mock.Of<IUserStore<User>>();
            userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);

            repository = new Mock<IRoleRepository>();

            var roleMappingProfile = new RoleMapping();
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(roleMappingProfile));
            mapper = new Mapper(mapperConfiguration);

            roleService = new RoleService(roleManager.Object, userManager.Object, repository.Object, mapper);
        }

        [TestClass]
        public class GetRoles : RoleServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfEmpty()
            {
                repository.Setup(r => r.GetRoles()).ReturnsAsync(new List<Role>());

                var result = await roleService.GetRoles();

                repository.Verify(r => r.GetRoles(), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfNotEmpty()
            {
                repository.Setup(r => r.GetRoles()).ReturnsAsync(new List<Role>() {
                    new Role(),
                    new Role()
                });

                var result = await roleService.GetRoles();

                repository.Verify(r => r.GetRoles(), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                repository.Setup(r => r.GetRoles()).ThrowsAsync(new Exception());

                var result = await roleService.GetRoles();

                repository.Verify(r => r.GetRoles(), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class AddRole : RoleServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await roleService.AddRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfRoleNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((Role)null);

                var result = await roleService.AddRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotSucceded()
            {
                var identityError = new IdentityError();
                identityError.Description = "Something went wrong";

                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                userManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(identityError));

                var result = await roleService.AddRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(rm => rm.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
                Assert.IsNotNull(result.Errors);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfSucceeded()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                userManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

                var result = await roleService.AddRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(rm => rm.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                userManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ThrowsAsync(new Exception());

                var result = await roleService.AddRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(rm => rm.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class CreateRole : RoleServiceTest
        {
            [TestMethod]
            public async Task ReturnsInvalidIfNotCreated()
            {
                var identityError = new IdentityError();
                identityError.Description = "Something went wrong";

                roleManager.Setup(rm => rm.CreateAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Failed(identityError));

                var result = await roleService.CreateRole(It.IsAny<string>());

                roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
                Assert.IsNotNull(result.Errors);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfCreated()
            {
                roleManager.Setup(rm => rm.CreateAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success);

                var result = await roleService.CreateRole(It.IsAny<string>());

                roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                roleManager.Setup(rm => rm.CreateAsync(It.IsAny<Role>())).ThrowsAsync(new Exception());

                var result = await roleService.CreateRole(It.IsAny<string>());

                roleManager.Verify(rm => rm.CreateAsync(It.IsAny<Role>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class RemoveRole : RoleServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfUserNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

                var result = await roleService.RemoveRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsNotFoundIfRoleNotFound()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((Role)null);

                var result = await roleService.RemoveRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotSucceded()
            {
                var identityError = new IdentityError();
                identityError.Description = "Something went wrong";

                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                userManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(identityError));

                var result = await roleService.RemoveRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(rm => rm.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
                Assert.IsNotNull(result.Errors);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfSucceeded()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                userManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

                var result = await roleService.RemoveRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(rm => rm.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                userManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ThrowsAsync(new Exception());

                var result = await roleService.RemoveRole(It.IsAny<string>(), It.IsAny<string>());

                userManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                userManager.Verify(rm => rm.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }

        [TestClass]
        public class DeleteRole : RoleServiceTest
        {
            [TestMethod]
            public async Task ReturnsNotFoundIfRoleNotFound()
            {
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((Role)null);

                var result = await roleService.DeleteRole(It.IsAny<int>());

                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsInvalidIfNotSucceeded()
            {
                var identityError = new IdentityError();
                identityError.Description = "Something went wrong";

                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                roleManager.Setup(rm => rm.DeleteAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Failed(identityError));

                var result = await roleService.DeleteRole(It.IsAny<int>());

                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.DeleteAsync(It.IsAny<Role>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Invalid, result.ResultType);
                Assert.IsNotNull(result.Errors);
            }

            [TestMethod]
            public async Task ReturnsSuccessIfSucceeded()
            {
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                roleManager.Setup(rm => rm.DeleteAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success);

                var result = await roleService.DeleteRole(It.IsAny<int>());

                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.DeleteAsync(It.IsAny<Role>()), Times.Once);

                Assert.IsNotNull(result.Data);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }

            [TestMethod]
            public async Task ReturnsUnexpectedIfThrowsException()
            {
                roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new Role());
                roleManager.Setup(rm => rm.DeleteAsync(It.IsAny<Role>())).ThrowsAsync(new Exception());

                var result = await roleService.DeleteRole(It.IsAny<int>());

                roleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
                roleManager.Verify(rm => rm.DeleteAsync(It.IsAny<Role>()), Times.Once);

                Assert.IsNull(result.Data);
                Assert.AreEqual(ResultType.Unexpected, result.ResultType);
            }
        }
    }
}