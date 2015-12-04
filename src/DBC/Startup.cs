using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DBC.Models;
using DBC.Services;
using Anderman.TagHelpers;
using DBC.Logging;
using System.Globalization;
using DBC.Models.DB;
using Microsoft.AspNet.Localization;
using Anderman.JsonLocalization.Middelware;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage.Internal;

namespace DBC
{
    public class StartupTest
    {
        private Startup _startup;
        private IHostingEnvironment _hostingEnv;
        public IServiceCollection Services;
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _hostingEnv = services.Where(m => m.ServiceType == typeof(IHostingEnvironment) && m.ImplementationInstance != null).Select(m => m.ImplementationInstance).Last() as IHostingEnvironment;
            _startup = new Startup(_hostingEnv);

            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>(o=>o.UseInMemoryDatabase());

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            LocalizationServiceCollectionJsonExtensions.AddLocalization(services);
            services
                .AddMvc(m =>
                {
                    m.ModelMetadataDetailsProviders.Add(new AdditionalValuesMetadataProvider());
                });

            // Add application services.

            services.Configure<MessageServicesOptions>(_startup.Configuration.GetSection("mailSettings"));
            services.AddSingleton<IEmailSender, MessageServices>();
            services.AddSingleton<ISmsSender, MessageServices>();
            services.AddTransient<IEmailTemplate, EmailTemplate>();
            Services = services;
            return Services.BuildServiceProvider();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _startup.Configure(app, env, loggerFactory);
            app.EnsureSampleData().Wait();
        }
    }
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            }
            builder.AddUserSecrets();

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Tokens.ChangeEmailTokenProvider = "";
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            LocalizationServiceCollectionJsonExtensions.AddLocalization(services);
            //services.TryAddTransient<IMvcRazorHost, MyMvcRazorHost>();
            services
                .AddMvc(m =>
                {
                    m.ModelMetadataDetailsProviders.Add(new AdditionalValuesMetadataProvider());
                })
                .AddViewLocalization(options => options.ResourcesPath = "Resources")
                //.AddDataAnnotationsLocalization()
                ;

            // Add application services.

            services.Configure<MessageServicesOptions>(Configuration.GetSection("mailSettings"));
            services.AddSingleton<IEmailSender, MessageServices>();
            services.AddSingleton<ISmsSender, MessageServices>();
            services.AddTransient<IEmailTemplate, EmailTemplate>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (env.EnvironmentName == "Testing")
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch (Exception ex) { }
            }
            app.UseRequestLocalization(new RequestLocalizationOptions()
            {
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("nl-NL")
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("nl-NL")
                },
            }, new RequestCulture(new CultureInfo("nl-NL")));

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715
            if (Configuration["Authentication:Google:ClientId"] != null)
            {
                app.UseFacebookAuthentication(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    options.DisplayName = "facebook";
                });
                app.UseGoogleAuthentication(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    options.DisplayName = "google plus";
                });
            }

            app.UseCacheWrite();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
