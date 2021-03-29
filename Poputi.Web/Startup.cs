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

            // Сервисы работы с геометрией.
            services.AddSingleton(new NtsGeometryServices());
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
