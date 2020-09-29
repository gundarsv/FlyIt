using FlyIt.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlyIt.DataAccess
{
    public class IdentityContext : IdentityDbContext<User, Role, int ,UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<UserLogin>().ToTable("UserLogin");
            builder.Entity<UserToken>().ToTable("UserToken");
            builder.Entity<UserClaim>().ToTable("UserClaim");
            builder.Entity<UserRole>().ToTable("UserRole");
            builder.Entity<RoleClaim>().ToTable("RoleClaim");
            builder.Entity<Role>().ToTable("Role");
        }
    }
}
