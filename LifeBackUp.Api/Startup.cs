using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.S3;
using LifeBackUp.Core.Interfaces;
using LifeBackUp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace LifeBackUp.Api
{
    public class Startup
    {

        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>(_configuration.GetAWSOptions());
            services.AddSingleton<IBucketRepository, BucketRepository>();
            services.AddSingleton<IFilesRepository, FilesRepository>();

            services.AddMvc(option => option.EnableEndpointRouting = false);
            AddSwaggerSupport(services);
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = JsonConvert.SerializeObject(new {error = exception.Message});
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LifeBackup API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();

        }

        private void AddSwaggerSupport(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "LifeBackup API",
                    Description = "Interact with Amazon Web Services (AWS) S3 with ASP.NET Core and the AWS SDK for .NET",
                    Contact = new OpenApiContact
                    {
                        Name = "Rathin Padia",
                        Email = "rathinpadia4u@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/rathin-padia-264b8610/"),
                    }

                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
        }
    }
}
