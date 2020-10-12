using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FlyIt.Api.Infrastructure
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
            {
                Name = "Bearer",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
            };
            OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Id = "access_token",
                    Type = ReferenceType.SecurityScheme
                }
            };
            OpenApiSecurityRequirement securityRequirements = new OpenApiSecurityRequirement()
            {
                {
                    securityScheme, new string[] { }
                },
            };

            return services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("access_token", securityDefinition);
                c.AddSecurityRequirement(securityRequirements);
            });
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder application)
        {
            return application
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
        }
    }
}