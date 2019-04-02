using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ncs.Prototype.Web.Web1.Data;
using Ncs.Prototype.Web.Web1.Services;

namespace Ncs.Prototype.Web.Web1
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<ICourseService, CourseService>();

            services.AddHealthChecks()
                .AddCheck<HealthCheck.Web1HealthCheck>(typeof(HealthCheck.Web1HealthCheck).Name);

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
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
