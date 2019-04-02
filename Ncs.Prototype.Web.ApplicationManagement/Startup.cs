using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ncs.Prototype.Common;
using Ncs.Prototype.Interfaces;
using Ncs.Prototype.Web.ApplicationManagement.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace Ncs.Prototype.Web.ApplicationManagement
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private const string Name = "Application Management";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cosmosSettings = Configuration.GetSection("cosmos").Get<CosmosSettings>();
            services.AddSingleton(cosmosSettings);

            services.AddTransient<ApplicationService>();
            services.AddSingleton<IStorage>(new CosmosDbStorage(cosmosSettings.Uri, cosmosSettings.Key));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = Name,
                    Version = "v1",
                    Description = Name
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ConfigureSwagger(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
        private void ConfigureSwagger(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", Name);
                x.RoutePrefix = string.Empty;
            });
        }
    }
}
