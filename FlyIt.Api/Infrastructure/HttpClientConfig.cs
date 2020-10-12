using FlyIt.Domain.Services;
using FlyIt.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FlyIt.Api.Infrastructure
{
    public static class HttpClientConfig
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, AviationstackSettings settings)
        {
            services.AddHttpClient<IAviationstackFlightService, AviationstackFlightService>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl + settings.ApiVersion + "/flights?access_key=" + settings.AccessKey);
            });

            return services;
        }
    }
}