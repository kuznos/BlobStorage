using Azure.Storage.Blobs;
using BlobStorage.Domain.Enums;
using BlobStorage.Domain.Helpers;
using BlobStorage.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlobStorage.API
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

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                    });
            });

            services.AddControllers();
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.DefaultApiVersion = ApiVersion.Default;
            });

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlobStorage API v1",
                    Description = "API v1 to perform Azure BlobStorage operations",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "kuznos IT Department",
                        Email = "it@kuznos.gr",
                        Url = new Uri("https://www.kuznos.gr"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CC BY-SA",
                        Url = new Uri("https://creativecommons.org/licenses/by-sa/4.0/"),
                    }
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "BlobStorage API v2",
                    Description = "API v2 to perform Azure BlobStorage operations",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "kuznos IT Department",
                        Email = "it@kuznos.gr",
                        Url = new Uri("https://www.kuznos.gr"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CC BY-SA",
                        Url = new Uri("https://creativecommons.org/licenses/by-sa/4.0/"),
                    }
                });

                //c.ExampleFilters();
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


            //------------------ AppEnvironment ---------------------------
            AppEnvironment appEnv = AppEnvironment.Dev;

            //------------------ AppEnvironment ---------------------------

            //services.AddSwaggerExamplesFromAssemblyOf<Startup>();
            //services.AddSingleton(x => new BlobServiceClient(connectionString: Configuration.GetValue<string>(key: "AzureBlobStorageConnectionString")));

            try
            {
                services.AddSingleton(x => new BlobServiceClient(Global_Helper.GetAzureBlobStorageConnStrFromAzureKeyVault(appEnv)));               
                services.AddSingleton<IBlobService, BlobService>();
                services.AddHealthChecks();
            }
            catch (Exception ex)
            {
                Global_Helper.SendEmailAsync("Startup ConfigureServices " + "\r\n" +  ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }

          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseStaticFiles();
                app.UseSwaggerUI(
                options =>
                {                  
                    options.DocumentTitle = "kuznos - Azure BlobStorage API";
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "BlobStorage API Version : " + description.GroupName);
                    }
                });

            }

            //For Public Use enable below
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResultStatusCodes =
                        {
                            [HealthStatus.Healthy] = StatusCodes.Status200OK,
                            [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                            [HealthStatus.Unhealthy] = StatusCodes.Status404NotFound,
                            [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError
                        }
                });
            });


          

        }
    }
}
