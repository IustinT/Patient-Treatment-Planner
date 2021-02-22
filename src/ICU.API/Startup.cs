using FluentValidation.AspNetCore;

using ICU.API.Models.FluentValidation;
using ICU.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;

[assembly: ApiController]

namespace ICU.API
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
            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
            })
                .AddNewtonsoftJson()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);

                        result.ContentTypes.Add(MediaTypeNames.Application.Json);

                        return result;
                    };
                })
                .AddFluentValidation(
                    fv => fv.RegisterValidatorsFromAssemblyContaining<PatientValidator>()
                );

            services.AddDbContext<IcuContext>(OptionsAction);


            services.AddSwaggerGen();

        }

        static void OptionsAction(DbContextOptionsBuilder options) =>
            options.UseSqlServer("name=ConnectionStrings:DefaultConnection");


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultures = new List<CultureInfo> { new CultureInfo("en-GB") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(cultures[0]),
                SupportedCultures = cultures,
                SupportedUICultures = cultures
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHttpsRedirection();
            }

            app.UseSerilogRequestLogging();
            //app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/ICU.API/swagger/v1/swagger.json", "ICU API V1");
                c.RoutePrefix = string.Empty;

            });

            app.UsePathBase("/api");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}