using FlyIt.DataAccess.Entities.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface IRoleRepository
    {
        public Task<List<Role>> GetRoles();
    }
}
