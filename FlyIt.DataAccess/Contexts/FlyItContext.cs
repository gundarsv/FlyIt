using FlyIt.DataAccess.Entities;
using FlyIt.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlyIt.DataAccess
{
    public class FlyItContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Flight> Flight { get; set; }

        public DbSet<Airport> Airport { get; set; }

        public DbSet<News> News { get; set; }

        public DbSet<UserFlight> UserFlight { get; set; }

        public DbSet<UserAirport> UserAirport { get; set; }

        public DbSet<UserToken> UserToken { get; set; }

        public DbSet<Chatroom> Chatroom { get; set; }

        public DbSet<UserChatroom> UserChatroom { get; set; }

        public DbSet<ChatroomMessage> ChatroomMessage { get; set; }

        public FlyItContext(DbContextOptions<FlyItContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<UserToken>().ToTable("UserToken");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRole");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");
            builder.Entity<Role>().ToTable("Role");

            builder.Entity<UserAirport>()
                .HasKey(ua => new { ua.UserId, ua.AirportId });

            builder.Entity<UserAirport>()
               .HasOne(ua => ua.User)
               .WithMany(u => u.UserAirports)
               .HasForeignKey(ua => ua.UserId);

            builder.Entity<UserAirport>()
               .HasOne(ua => ua.Airport)
               .WithMany(a => a.UserAirports)
               .HasForeignKey(ua => ua.AirportId);

            builder.Entity<UserFlight>()
               .HasKey(uf => new { uf.UserId, uf.FlightId });

            builder.Entity<UserFlight>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.UserFlights)
                .HasForeignKey(uf => uf.UserId);

            builder.Entity<UserFlight>()
                .HasOne(uf => uf.Flight)
                .WithMany(f => f.UserFlights)
                .HasForeignKey(uf => uf.FlightId);

            builder.Entity<Airport>()
                .HasMany(a => a.News)
                .WithOne(n => n.Airport)
                .HasForeignKey(n => n.AirportId);

            builder.Entity<UserToken>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<UserToken>()
               .HasKey(ut => new { ut.RefreshToken, ut.Id });

            builder.Entity<UserToken>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTokens)
                .HasForeignKey(ut => ut.UserId);

            builder.Entity<UserChatroom>()
               .HasKey(ucr => new { ucr.UserId, ucr.ChatroomId });

            builder.Entity<UserChatroom>()
               .HasOne(ucr => ucr.Chatroom)
               .WithMany(cr => cr.UserChatrooms)
               .HasForeignKey(ucr => ucr.ChatroomId);

            builder.Entity<UserChatroom>()
               .HasOne(ucr => ucr.User)
               .WithMany(u => u.UserChatrooms)
               .HasForeignKey(ucr => ucr.UserId);

            builder.Entity<Chatroom>()
               .Property(cr => cr.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<Chatroom>()
                .HasOne(cr => cr.Flight)
                .WithOne(f => f.Chatroom)
                .HasForeignKey<Chatroom>(cr => cr.FlightId);

            builder.Entity<ChatroomMessage>()
               .Property(crm => crm.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<ChatroomMessage>()
                .HasOne(crm => crm.Chatroom)
                .WithMany(cr => cr.ChatroomMessages)
                .HasForeignKey(cr => cr.ChatroomId);

            builder.Entity<ChatroomMessage>()
                .HasOne(crm => crm.User)
                .WithMany(u => u.ChatroomMessages)
                .HasForeignKey(u => u.UserId);
        }
    }
}