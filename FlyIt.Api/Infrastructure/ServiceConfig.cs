﻿using FlyIt.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlyIt.Api.Infrastructure
{
    public static class ServiceConfig
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IGoogleCloudStorageService, GoogleCloudStorageService>();
            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<IChatService, ChatService>();

            return services;
        }
    }
}