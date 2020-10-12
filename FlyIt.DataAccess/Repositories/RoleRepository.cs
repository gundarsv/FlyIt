using FlyIt.DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly FlyItContext context;

        public RoleRepository(FlyItContext context)
        {
            this.context = context;
        }

        public async Task<List<Role>> GetRoles()
        {
            return await context.Roles.ToListAsync();
        }
    }
}