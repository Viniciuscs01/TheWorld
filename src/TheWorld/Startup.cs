﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            services.AddMvc();
            //.AddJsonOptions(config =>
            //{
            //    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});

            services.AddIdentity<WorldUser, IdentityRole>()
            .AddEntityFrameworkStores<WorldContext>()
            .AddDefaultTokenProviders();

            services.AddLogging();

            services.AddEntityFrameworkSqlServer().AddDbContext<WorldContext>();

            services.AddTransient<GeoCoordsServices>();
            services.AddTransient<WorldContextSeedData>();
            services.AddScoped<IWorldRepository, WorldRepository>();

#if DEBUG
            services.AddScoped<IMailService, DebugMailService>();
#else
            services.AddScoped<IMailService, MailService>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, WorldContextSeedData seeder)
        {
            loggerFactory.AddDebug(LogLevel.Warning);
            app.UseStaticFiles();
            app.UseIdentity();

            Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>().ReverseMap();
                config.CreateMap<StopViewModel, Stop>().ReverseMap();
            });

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" });
            });

            seeder.EnsureSeedData().Wait();
        }
    }
}
