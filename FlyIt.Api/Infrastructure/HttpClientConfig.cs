using FlyIt.Domain.Services;
using FlyIt.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FlyIt.Api.Infrastructure
{
    public static class HttpClientConfig
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, AviationstackSettings settings, CheckWXAPIConfigSettings settingsWeather)
        {
            services.AddHttpClient<IAviationstackFlightService, AviationstackFlightService>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseUrl + settings.ApiVersion + "/flights?access_key=" + settings.AccessKey);
            });

            services.AddHttpClient<ICheckWXAPIMetarService, CheckWXAPIMetarService>(client =>
            {
                client.BaseAddress = new Uri(settingsWeather.BaseUrl);
                client.DefaultRequestHeaders.Add("X-API-Key", settingsWeather.AccessKey);
            });

            services.AddHttpClient<ICheckWXAPIStationService, CheckWXAPIStationService>(client =>
            {
                client.BaseAddress = new Uri(settingsWeather.BaseUrl);
                client.DefaultRequestHeaders.Add("X-API-Key", settingsWeather.AccessKey);
            });

            return services;
        }
    }
}