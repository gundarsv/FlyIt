using FlyIt.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace FlyIt.Api.Infrastructure
{
    public static class HelperConfig
    {
        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddScoped<JWTHelper, JWTHelper>();
            return services;
        }
    }
}
