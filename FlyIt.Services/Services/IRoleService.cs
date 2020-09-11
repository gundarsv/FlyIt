using FlyIt.DataContext.Entities.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public interface IRoleService
    {
        public Task<IActionResult> CreateRole(string roleName);
        public Task<IActionResult> AddRole(string email, string roleName);
    }
}
