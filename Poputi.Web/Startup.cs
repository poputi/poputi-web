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
                // ����� ������� � launchSettings.json � ���������� ����� ��� ������ "ConnectionStrings:DefaultConnection" ��� ��������� ������.
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                    // ���������� ��� ������ � ����������������� �������.
                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite(geographyAsDefault: true)
                    );
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // ��������� ���������� ���������.
                    // this constructor is overloaded.  see other overloads for options.
                    var geoJsonConverterFactory = new GeoJsonConverterFactory();
                    options.JsonSerializerOptions.Converters.Add(geoJsonConverterFactory);
                });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IDriverService, DriverService>();

            // ������� ������ � ����������.
            services.AddSingleton(new NtsGeometryServices());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "������",
                    Description = "�������� ������� ��� ������ � API \"������\""
                });

                /*
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath); 
                */
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

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "������");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
