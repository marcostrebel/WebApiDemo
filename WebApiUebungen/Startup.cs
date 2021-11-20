using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Services;
using WebApiUebungen.Settings;

namespace WebApiUebungen
{
    public class Startup
    {
        public AppSettings AppSettings { get; set; } = new();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Configuration.Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebApiDemoDbContext>(options =>
            {
                var serverBuilder = options.UseSqlServer(AppSettings.Database.ConnectionString);
                serverBuilder.LogTo(Console.WriteLine).EnableSensitiveDataLogging();
            });

            services.ConfigureDefaultAuthentication(AppSettings.Authentication)
                .ConfigureJwtAuthentication(AppSettings.Authentication.Jwt);

            services.AddSingleton(AppSettings.Authentication.Jwt);

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApiUebungen",
                    Version = "v1",
                    Description = "Web API Übungsprojekt (DAS .NET, OST Rapperswil)"
                });

                c.IncludeXmlComments("WebApiUebungen.xml");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                System.Diagnostics.Debug.WriteLine("Before next...");

                context.Response.Headers.Add("Test-Header", "Hello World");

                await next();

                System.Diagnostics.Debug.WriteLine("After next...");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiUebungen v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCustomExceptionHandlerMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
