using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DBC.Controllers;
using DBC.Models.DB;
using DBC.Services;
using DBC.test.TestApplication;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Moq;
using Xunit;
using Roles = DBC.Models.DB.Roles;

namespace DBC.test.Controllers
{
    public class AccountWithServicesTest
    {
        private async void BuildSampleData(IServiceProvider service)
        {
            service.GetRequiredService<IInMemoryStore>().Clear();
            var db = service.GetRequiredService<ApplicationDbContext>();
            if (db.Database != null && db.Database.EnsureCreated())
            {
                var userManager = service.GetService<UserManager<ApplicationUser>>();
                var roleManager = service.GetService<RoleManager<IdentityRole>>();

                await CreateUserIfNotExist(userManager, "Bobbie@kuifje.be", "@password!", Roles.Admin.ToString());
            }
        }

        private static async Task CreateUserIfNotExist(UserManager<ApplicationUser> userManager, string email, string password, string role, string loginProvider = null, string providerKey = null)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser {UserName = email, Email = email};
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new ApplicationException(string.Join("\n", result.Errors.Select(a => a.Description).ToArray()));
                }
                await userManager.AddToRoleAsync(user, role);
                if (loginProvider != null && providerKey != null)
                {
                    await userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, ""));
                }
            }
        }

        [Fact]
        public void Login_succes()
        {
            // Arrange
            var req = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request).Returns(req.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);


            Func<IServiceCollection, IServiceProvider> applicationServices = services =>
            {
                services.AddInstance(httpContextAccessor.Object);
                services.AddEntityFramework()
                    .AddInMemoryDatabase()
                    .AddDbContext<ApplicationDbContext>();

                services.AddMvc();
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
                services.AddSingleton<IEmailSender, MessageServices>();
                services.AddSingleton<ISmsSender, MessageServices>();
                services.AddTransient<IEmailTemplate, EmailTemplate>();
                services.AddTransient<AccountController, AccountController>();
                LocalizationServiceCollectionJsonExtensions.AddLocalization(services);
                return services.BuildServiceProvider();
            };

            var buildServices = InitializeServices(typeof (StartupTest).Assembly, applicationServices);
            var serviceProvider = buildServices(new ServiceCollection());
            //BuildSampleData(serviceProvider);
            var accountController = serviceProvider.GetRequiredService<AccountController>();
            var response = accountController.Login() as ViewResult;
            var actionContext = GetActionContext();
            //await response.ExecuteResultAsync(b.ActionContext);
        }

        public Func<IServiceCollection, IServiceProvider> InitializeServices(Assembly startupAssembly, Func<IServiceCollection, IServiceProvider> buildServices)
        {
            var applicationName = startupAssembly.GetName().Name;
            var applicationRoot = GetApplicationRoot(applicationName);
#if DNX451
            AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", applicationRoot);
#endif
            var applicationEnvironment = PlatformServices.Default.Application;

            var hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.Initialize(applicationRoot, null);

            var assemblyProvider = new StaticAssemblyProvider();
            assemblyProvider.CandidateAssemblies.Add(startupAssembly);

            return services =>
            {
                services.AddInstance<IApplicationEnvironment>(new TestApplicationEnvironment(applicationEnvironment, applicationName, applicationRoot));
                services.AddInstance<IHostingEnvironment>(hostingEnvironment);
                // Inject a custom assembly provider. Overrides AddMvc() because that uses TryAdd().
                services.AddInstance<IAssemblyProvider>(assemblyProvider);

                return buildServices(services);
            };
        }

        public static string GetApplicationRoot(string applicationName) =>
            Path.GetDirectoryName(PlatformServices.Default.LibraryManager.GetLibrary(applicationName).Path);

        public static ActionContext GetActionContext(string controller = "mycontroller", string area = null)
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = controller;
            if (area != null)
            {
                routeData.Values["area"] = area;
            }

            var actionDesciptor = new ActionDescriptor();
            actionDesciptor.RouteConstraints = new List<RouteDataActionConstraint>();
            return new ActionContext(new DefaultHttpContext(), routeData, actionDesciptor);
        }
    }
}