using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface IRoleService
    {
        public Task<Result<IdentityResult>> CreateRole(string roleName);

        public Task<Result<IdentityResult>> DeleteRole(int id);

        public Task<Result<IdentityResult>> AddRole(string userId, string roleId);

        public Task<Result<IdentityResult>> RemoveRole(string userId, string roleId);

        public Task<Result<List<RoleDTO>>> GetRoles();
    }
}