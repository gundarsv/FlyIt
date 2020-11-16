using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Test.Repositories
{
    public class RoleRepositoryTest
    {
        private readonly FlyItContext flyItContext;
        private readonly RoleRepository repository;

        public RoleRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<FlyItContext>()
                .UseInMemoryDatabase(databaseName: "FlyIt-Role")
                .Options;

            flyItContext = new FlyItContext(options);

            repository = new RoleRepository(flyItContext);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            flyItContext.RemoveRange(flyItContext.Roles);

            await flyItContext.SaveChangesAsync();
        }

        [TestClass]
        public class GetRoles : RoleRepositoryTest
        {
            [TestMethod]
            public async Task CanGetRoles()
            {
                var role = new Role()
                {
                    Name = "Role"
                };

                await flyItContext.AddAsync(role);

                await flyItContext.SaveChangesAsync();

                var result = await repository.GetRoles();

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count > 0);
            }

            [TestMethod]
            public async Task ReturnsEmptyIfDoesNotExist()
            {
                var result = await repository.GetRoles();

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count == 0);
            }
        }
    }
}