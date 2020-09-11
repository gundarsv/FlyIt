using FlyIt.DataContext.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlyIt.DataContext
{
    public class IdentityContext : IdentityDbContext<User, Role, int>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
