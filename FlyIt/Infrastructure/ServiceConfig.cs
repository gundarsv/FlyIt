using FlyIt.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlyIt.Api.Infrastructure
{
    public static class ServiceConfig
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
