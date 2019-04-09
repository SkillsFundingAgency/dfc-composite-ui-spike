using CorrelationId;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using Ncs.Prototype.Web.Composition.ChangeFeedServices;
using Ncs.Prototype.Web.Composition.Data;
using Ncs.Prototype.Web.Composition.Framework;
using Ncs.Prototype.Web.Composition.HealthCheck;
using Ncs.Prototype.Web.Composition.Loggers;
using Ncs.Prototype.Web.Composition.Options;
using Ncs.Prototype.Web.Composition.Services;
using System;
using System.Linq;

namespace Ncs.Prototype.Web.Composition
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json");

            Configuration = configBuilder.Build();            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelationId();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var apiManagementConfiguration = Configuration.GetSection("ApiManagement").Get<ApiManagementConfigurationDto>();
            services.AddSingleton(apiManagementConfiguration);

            var cosmosSettings = Configuration.GetSection("cosmos").Get<CosmosSettings>();
            services.AddSingleton(cosmosSettings);

            services.AddSingleton<ILog, FileLog>();
            services.AddScoped<ApplicationManagementService>();
            services.AddSingleton<IHostedService, CosmosDBHostedBackgroundService>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("option2"));

            services.AddDefaultIdentity<IdentityUser>()
                //        .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    "Google",
                    options =>
                    {
                        options.Authority = Configuration["TokenProviderOptions:Authority"];
                        options.ClientId = Configuration["TokenProviderOptions:ClientId"];
                        options.ClientSecret = Configuration["TokenProviderOptions:ClientSecret"];
                        options.ResponseType = "code id_token";
                        options.Scope.Add("email");
                        options.Scope.Add("address");
                        options.GetClaimsFromUserInfoEndpoint = true;
                        options.SaveTokens = true;
                    });

            //services
            //   .AddAuthentication()
            //   .AddGoogle(o =>
            //   {
            //       o.ClientSecret = Configuration["TokenProviderOptions:ClientSecret"];
            //       o.ClientId = Configuration["TokenProviderOptions:ClientId"];
            //       o.UserInformationEndpoint = Configuration["TokenProviderOptions:UserInformationEndpoint"];
            //       o.Scope.Add("https://www.googleapis.com/auth/plus.login");

            //       o.ClaimActions.Clear();
            //       o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            //       o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            //       o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            //       o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
            //       o.ClaimActions.MapJsonKey("urn:google:profile", "profile");
            //       o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            //       o.ClaimActions.MapJsonKey("picture", "picture");
            //       o.ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");

            //       o.SaveTokens = true;
            //   });

            var sp = services.BuildServiceProvider();
            var applicationManagementService = sp.GetService<ApplicationManagementService>();

            AddHealthChecks(services, applicationManagementService);

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationManagementService applicationManagementService)
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
            app.UseAuthentication();

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

            ConfigureRouting(app, applicationManagementService);
        }

        private void AddHealthChecks(IServiceCollection services, ApplicationManagementService applicationManagementService)
        {
            services.AddHealthChecksUI();
            var healthChecks = services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
            var applications = applicationManagementService.GetApplications().Result;

            var canCheck = applications.Where(w => !string.IsNullOrEmpty(w.HealthCheckUrl)).ToList();

            canCheck.ForEach(f => healthChecks.AddUrlGroup(new Uri(f.HealthCheckUrl), name: f.Name, tags: new string[] { $"{f.RouteName}-api" }));

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

        private void ConfigureRouting(IApplicationBuilder app, ApplicationManagementService applicationManagementService)
        {
            // This isnt ideal...
            var applications = applicationManagementService.GetApplications().Result;

            app.UseMvc(routes =>
            {
                // map any incoming routes for each application
                foreach (var application in applications.Where(w => !string.IsNullOrEmpty(w.MainMenuText)))
                {
                    if (application.RequiresAuthorization)
                    {
                        routes.MapRoute(
                             name: $"Authorized-{application.RouteName}-Action",
                             template: application.RouteName + "/{**data}",
                             defaults: new { controller = "Authorized", action = "Action", application.RouteName }
                        );
                    }
                    else
                    {
                        routes.MapRoute(
                            name: $"Application-{application.RouteName}-Action",
                             template: application.RouteName + "/{**data}",
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
    }
}
