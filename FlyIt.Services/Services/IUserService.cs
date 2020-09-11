using FlyIt.DataContext.Entities.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlyIt.Services.Services
{
    public interface IUserService
    {
        public Task<IActionResult> CreateUser(User user, string password);
        public Task<IActionResult> SignInUser(string userName, string password);
    }
}
