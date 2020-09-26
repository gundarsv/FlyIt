using Microsoft.AspNetCore.Identity;

namespace FlyIt.DataAccess.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
    }
}
