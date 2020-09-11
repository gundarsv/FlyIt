using FlyIt.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
