using AutoMapper;
using FlyIt.Api.Infrastructure;
using FlyIt.DataAccess;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Settings;
using Google.Apis.Auth.OAuth2;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FlyIt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FlyItContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"))
                .UseLazyLoadingProxies()
                .LogTo(Console.WriteLine));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<FlyItContext>()
                .AddDefaultTokenProviders();

            services.Configure<JWTSettings>(Configuration.GetSection("JWTConfig"));

            services.Configure<GoogleCloudSettings>(Configuration.GetSection("GoogleCloudConfig"));

            services.Configure<GoogleCredential>(Configuration.GetSection("GoogleCloudConfig.GoogleCloudKey"));

            services.AddAuth(Configuration.GetSection("JWTConfig").Get<JWTSettings>());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddWrappers();

            services.AddRepositories();

            services.AddHttpClients(Configuration.GetSection("AviationstackConfig").Get<AviationstackSettings>(), Configuration.GetSection("CheckWXAPIConfig").Get<CheckWXAPIConfigSettings>());

            services.AddSingleton<ICompareLogic, CompareLogic>();

            services.AddServices();

            services.AddSwaggerServices();

            services.AddCors();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FlyItContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseAuth();

            dataContext.Database.Migrate();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}