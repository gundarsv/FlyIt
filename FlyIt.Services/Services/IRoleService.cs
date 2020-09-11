using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public interface IRoleService
    {
        public Task<IActionResult> CreateRole(string roleName);
        public Task<IActionResult> AddRole(string email, string roleName);
    }
}
