using System;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using Ncs.Prototype.Web.Courses.Services;

namespace Ncs.Prototype.Web.Courses
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
            ConfigureApplicationRegistration(services);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<ICourseService, CourseService>();

            services.AddHealthChecks()
                .AddCheck<HealthCheck.CourseHealthCheck>(typeof(HealthCheck.CourseHealthCheck).Name);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, cfg =>
                {
                    cfg.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateIssuerSigningKey = true,

                                /*
                                 * if ValidateLifetime is set to true and the jwt is expired according to to both the ClockSkew and also the expiry on the jwt,then token is invalid
                                 * This will mark the User.IsAuthenticated as false
                                */
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromMinutes(1),

                                ValidIssuer = Configuration["TokenProviderOptions:Issuer"],
                                ValidAudience = Configuration["TokenProviderOptions:ClientId"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenProviderOptions:ClientSecret"]))
                            };
                    cfg.Authority = Configuration["TokenProviderOptions:Issuer"];
                    cfg.Events =
                        new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                return Task.CompletedTask;
                            },

                            OnMessageReceived = context =>
                            {
                                return Task.CompletedTask;
                            },
                            OnChallenge = context =>
                            {
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                return Task.CompletedTask;
                            }
                        };
                });

            //   services.AddAuthorization();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseHealthChecks("/ready", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                // add the courses routing
                routes.MapRoute(
                    name: $"Course-Index-Category",
                    template: "Course/Category/{category}",
                    defaults: new { controller = "Course", action = "Index" }
                );
                routes.MapRoute(
                    name: $"Course-Index-Filter",
                    template: "Course/Filter/{filter}",
                    defaults: new { controller = "Course", action = "Index" }
                );
                routes.MapRoute(
                    name: $"Course-Index",
                    template: "Course/Index",
                    defaults: new { controller = "Course", action = "Index" }
                );
                routes.MapRoute(
                    name: $"Course-Search",
                    template: "Course/Search/{searchClue?}",
                    defaults: new { controller = "Course", action = "Search" }
                );
                routes.MapRoute(
                    name: $"Course-Index-Search",
                    template: "Course/{searchClue}",
                    defaults: new { controller = "Course", action = "Index" }
                );

                // add the site map route
                routes.MapRoute(
                    name: "Sitemap",
                    template: "Sitemap",
                    defaults: new { controller = "Sitemap", action = "Sitemap" }
                );

                // add the default route
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureApplicationRegistration(IServiceCollection services)
        {
            var apiManagementConfiguration = Configuration.GetSection("ApiManagement").Get<ApiManagementConfigurationDto>();
            services.AddSingleton(apiManagementConfiguration);

            var applicationDto = Configuration.GetSection("Application").Get<ApplicationDto>();
            services.AddSingleton(applicationDto);

            services.AddAsyncInitializer<RegisterApplicationInitialiser>();
        }

    }
}
