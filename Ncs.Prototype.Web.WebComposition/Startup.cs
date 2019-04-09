using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorrelationId;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using Ncs.Prototype.Web.WebComposition.Framework;
using Ncs.Prototype.Web.WebComposition.HealthCheck;
using Ncs.Prototype.Web.WebComposition.Options;

namespace Ncs.Prototype.Web.WebComposition
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
            services.AddCorrelationId();

            ConfigureApplicationRegistration(services);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<Dto.RegisteredApplicationsDto>(Configuration.GetSection("RegisteredApplications"));
            services.Configure<Dto.ApplicationDto>(Configuration.GetSection("Application"));

            AddHealthChecks(services);

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

            //    services.AddAuthorization();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddPolicies(Configuration)
                .AddHttpClient<Services.IApplicationService, Services.ApplicationService, ApplicationClientOptions>(
                    Configuration,
                    nameof(ApplicationOptions.ApplicationClient)
                )
                .AddHttpClient<Services.IApplicationSitemapService, Services.ApplicationSitemapService, ApplicationClientOptions>(
                    Configuration,
                    nameof(ApplicationOptions.ApplicationClient)
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<Dto.RegisteredApplicationsDto> applications)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "X-Correlation-ID",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false               // = false is a Core 2.2 workaround - should be fixed in Core 2.2.2
            });

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

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                // map any incoming routes for each application
                foreach (var application in applications.Value.Applications.Where(w => !string.IsNullOrEmpty(w.MainMenuText)))
                {
                    if (application.RequiresAuthorization)
                    {
                        routes.MapRoute(
                             name: $"Authorized-{application.RouteName}-Action",
                             template: "Composite/" + application.RouteName + "/{**data}",
                             defaults: new { controller = "Authorized", action = "Action", application.RouteName }
                        );
                    }
                    else
                    {
                        routes.MapRoute(
                            name: $"Application-{application.RouteName}-Action",
                            template: "Composite/" + application.RouteName + "/{**data}",
                            defaults: new { controller = "Application", action = "Action", application.RouteName }
                        );
                    }
                }

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

        private void AddHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecksUI();
            var healthChecks = services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
            var applications = new Dto.RegisteredApplicationsDto();

            Configuration.Bind("RegisteredApplications", applications);

            var canCheck = applications.Applications.Where(w => !string.IsNullOrEmpty(w.HealthCheckUrl)).ToList();

            canCheck.ForEach(f => healthChecks.AddUrlGroup(new Uri(f.HealthCheckUrl), name: $"{f.RouteName}-check", tags: new string[] { $"{f.RouteName}-api" }));

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Period = new TimeSpan(0, 0, 4);
                options.Delay = TimeSpan.FromSeconds(2);
                options.Timeout = new TimeSpan(0, 0, 10);
                //    options.Predicate = (check) => check.Tags.Contains("ready");
            });

            const string HealthCheckServiceAssembly = "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckPublisherHostedService";

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton(typeof(IHostedService),
                    typeof(HealthCheckPublisherOptions).Assembly
                        .GetType(HealthCheckServiceAssembly)));

            services.AddSingleton<IHealthCheckPublisher, ReadinessPublisher>();
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
