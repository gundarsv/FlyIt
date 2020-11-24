using FlyIt.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FlyIt.Api.Infrastructure
{
    public static class RepositoryConfig
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<IFlightRepository, FlightRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAirportRepository, AirportRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IChatroomRepository, ChatroomRepository>();
            services.AddScoped<IChatroomMessageRepository, ChatroomMessageRepository>();
            services.AddScoped<IUserChatroomRepository, UserChatroomRepository>();

            return services;
        }
    }
}