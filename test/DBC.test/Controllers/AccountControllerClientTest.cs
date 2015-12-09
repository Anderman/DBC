using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DBC.Models.DB;
using DBC.Services;
using DBC.test.HtmlHelper;
using DBC.test.TestApplication;
using DBC.ViewModels.Account;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DBC.test.Controllers
{
    //public class AccountControllerClientTest1 : IClassFixture<TestServer2>
    //{
    //    public AccountControllerClientTest1(TestServer2 testServer2)
    //    {
    //        Client = testServer2.Client;
    //    }

    //    public HttpClient Client { get; private set; }

    //    [Fact]
    //    public async Task Login_Fail()
    //    {
    //        //Arrange
    //        var client = new ClientWrapper(Client);
    //        await client.Get("/Account/Login");
    //        //ACT
    //        await client.Post(formIndex: 2, defaults: new FormValues()
    //        {
    //            {"Email", "Bobbie@kuifje.be"},
    //            {"Password", "@Password!"}
    //        });
    //        //Assert
    //        Assert.Contains("Invalid login attempt", client.Html);
    //    }
    //}
    //public class TestServer2
    //{
    //    public HttpClient Client;

    //    public TestServer2()
    //    {

    //        Debugger.Launch();
    //        var MyServer = new TestServer(TestServer.CreateBuilder()
    //            .UseEnvironment("Development")
    //            .UseServer("DBC")
    //            .UseServices(services =>
    //            {
    //                services.AddInstance<IApplicationEnvironment>(new TestApplicationEnvironment(typeof(StartupTest).Assembly));

    //                services.AddInstance<IHostingEnvironment>(new TestHostingEnvironment(typeof(StartupTest).Assembly));

    //                services.AddEntityFramework()
    //               .AddInMemoryDatabase()
    //               .AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase());

    //                services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.SignIn.RequireConfirmedEmail = true; })
    //                    .AddEntityFrameworkStores<ApplicationDbContext>()
    //                    .AddDefaultTokenProviders();
    //                services
    //                    .AddMvc(m => { m.ModelMetadataDetailsProviders.Add(new AdditionalValuesMetadataProvider()); });
    //                LocalizationServiceCollectionJsonExtensions.AddLocalization(services);

    //                // Add application services.
    //                services.AddSingleton<IEmailSender, TestMessageServices>();
    //                services.AddSingleton<ISmsSender, MessageServices>();
    //                services.AddTransient<IEmailTemplate, EmailTemplate>();
    //            })
    //            .UseStartup<DBC.StartupTest>()
    //            );
    //        Client = MyServer.CreateClient();
    //    }
    //}

    public class AccountControllerClientTest : IClassFixture<TestApplicationFixture<StartupTest>>
    {

        public StartupTest Server { get; }
        public HttpClient Client { get; }
        public TestMessageServices TestMessageServices { get; }

        public AccountControllerClientTest(TestApplicationFixture<StartupTest> fixture)
        {
            Server = (StartupTest)fixture.Server;
            Client = fixture.Client;
            TestMessageServices = Server.ServiceProvider.GetRequiredService<IEmailSender>() as TestMessageServices;
        }




        [Fact]
        public async Task Login_Fail()
        {
            //Arrange
            var client = new ClientWrapper(Client);
            await client.Get("/Account/Login");
            //ACT
            await client.Post(2,
                new LoginViewModel
                {
                    Email = "Bobbie@kuifje.be",
                    Password = "@Pssw0rd!",
                });
            //Assert
            Assert.Contains("Invalid login attempt", client.Html);
        }

        [Fact]
        public async Task Login_Success_without_rememberMe()
        {
            //Arrange
            var client = new ClientWrapper(Client);
            await client.Get("/Account/Login");
            //ACT

            await client.Post(2,
                new LoginViewModel
                {
                    Email = "Bobbie@kuifje.be",
                    Password = "P@ssw0rd!",
                });
            Assert.Equal("/", client.AbsolutePath);
            Assert.Null(client.GetCookie(".AspNet.Microsoft.AspNet.Identity.Application").Expires);
        }

        [Fact]
        public async Task Login_Success_with_rememberMe()
        {
            //Arrange
            var client = new ClientWrapper(Client);
            await client.Get("/Account/Login");
            //ACT

            await client.Post(2,
                new LoginViewModel
                {
                    Email = "Bobbie@kuifje.be",
                    Password = "P@ssw0rd!",
                    RememberMe = true
                });
            Assert.Equal(null, client.ValidateResponse("/"));
            Assert.NotNull(client.GetCookie(".AspNet.Microsoft.AspNet.Identity.Application").Expires);
        }

        [Fact]
        public async Task Login_NotConfirmed()
        {
            var client = new ClientWrapper(Client);
            await client.Get("/Account/Login");
            //ACT

            await client.Post(2,
                new LoginViewModel
                {
                    Email = "confirm@test.nl",
                    Password = "P@ssw0rd!"
                });
            Assert.Contains("not confirmed", client.Html);
        }

        [Fact]
        public async Task Create_User_and_Login()
        {
            //Arrange
            string emailAddress = "new@test.nl";
            var client = new ClientWrapper(Client, TestMessageServices);
            //Act
            await client.Get("/User/Create");
            //Assert
            Assert.Equal(null, client.ValidateResponse("/Account/Login"));


            //ACT Account/Login
            await client.Post(2,
                new LoginViewModel
                {
                    Email = "thom@medella.nl",
                    Password = "P@ssw0rd!"
                });
            //Assert
            Assert.Equal(null, client.ValidateResponse("/User/Create"));


            //ACT post /User/Create
            await client.Post(1,
                new ApplicationUser
                {
                    UserName = emailAddress,
                    Email = emailAddress,
                    AccessFailedCount = 0,
                    EmailConfirmed = false,
                    TwoFactorEnabled = true
                });
            //Assert
            Assert.Contains("success", client.Html);
            Assert.True(client.HasEmail(emailAddress));


            //ACT get /Account/ConfirmEmail
            await client.ClickOnLinkInEmail(emailAddress);
            //Assert
            Assert.Equal(null, client.ValidateResponse("/Account/ConfirmEmail"));


            //ACT Post /Account/ConfirmEmail
            await client.Post(1,
                new ResetPasswordViewModel
                {
                    Password = "P@ssw0rd!",
                    ConfirmPassword = "P@ssw0rd!",
                    RememberMe = true
                }.AsFormValues()
                );
            //Assert
            Assert.Equal(null, client.ValidateResponse("/Account/SendCode"));
            Assert.Equal(null, client.ValidateForm(1,
                new SendCodeViewModel
                {
                    RememberMe = true
                }
                ));


            //ACT The post will send an securitycode and redirect to verifycode
            await client.Post(1, new FormValues());
            //Assert
            Assert.Equal(null, client.ValidateResponse("/Account/VerifyCode"));
            Assert.Equal(null, client.ValidateForm(1,
                new VerifyCodeViewModel
                {
                    RememberMe = true,
                    RememberBrowser = false
                }
                ));


            //ACT  post verify code
            var code = client.GetSecurityCode(emailAddress);
            await client.Post(1,
                new VerifyCodeViewModel
                {
                    Code = code,
                    RememberBrowser = true
                });
            //Assert
            Assert.Equal(null, client.ValidateResponse("/"));
        }
    }
}