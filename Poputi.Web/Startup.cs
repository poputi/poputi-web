using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Poputi.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite;
using Npgsql.NetTopologySuite;
using Poputi.DataAccess.Interfaces;
using Poputi.DataAccess.Services;
using NetTopologySuite.IO.Converters;
using Poputi.Logic.Interfaces;
using Poputi.Logic.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Poputi.Web.Auth;

// Все контроллеры в проекте теперь помечены как ApiController.
[assembly: ApiController]
namespace Poputi.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MainContext>(options =>
            {
                // Можно хранить в launchSettings.json в переменных среды под именем "ConnectionStrings:DefaultConnection" для локальной машины.
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                    // Надстройка для работы с пространственными данными.
                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite(geographyAsDefault: true)
                    );
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Настройка конвертера геометрии.
                    // this constructor is overloaded.  see other overloads for options.
                    var geoJsonConverterFactory = new GeoJsonConverterFactory();
                    options.JsonSerializerOptions.Converters.Add(geoJsonConverterFactory);
                });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IDriverService, DriverService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IdentityGenerator>();
            services.AddTransient<IRoutesService, RoutesService>();

            // Сервисы работы с геометрией.
            services.AddSingleton(new NtsGeometryServices());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Попути",
                    Description = "Описание методов для работы с API \"ПОПУТИ\""
                });

                /*
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath); 
                */
            });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidAudience = AuthOptions.AUDIENCE,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ПОПУТИ");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
