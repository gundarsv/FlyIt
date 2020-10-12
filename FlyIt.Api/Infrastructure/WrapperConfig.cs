using FlyIt.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlyIt.Api.Infrastructure
{
    public static class WrapperConfig
    {
        public static IServiceCollection AddWrappers(this IServiceCollection services)
        {
            services.AddScoped<IStorageClientWrapper, StorageClientWrapper>();

            return services;
        }
    }
}