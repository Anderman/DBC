using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Anderman.TagHelpers;
using DBC.Models.DB;
using DBC.Services;
using Microsoft.AspNet.TestHost;
using Xunit;
using DBC.test.HtmlHelpers;
using DBC.test.HtmlHelper;
using DBC.test.TestApplication;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNet.Hosting;

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
    //        await client.Post(formIndex: 2, defaults: new formValues()
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

    public class AccountControllerClientTest : IClassFixture<TestApplicationFixture<DBC.StartupTest>>
    {
        public StartupTest Server { get; }
        public TestMessageServices TestMessageServices { get; }

        public HttpClient Client { get; }
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
            await client.Post(formIndex: 2, defaults: new formValues()
            {
                {"Email", "Bobbie@kuifje.be"},
                {"Password", "@Password!"}
            });
            //Assert
            Assert.Contains("Invalid login attempt", client.Html);

        }

        [Fact]
        public async Task Login_Success()
        {
            //Arrange
            var client = new ClientWrapper(Client);
            await client.Get("/Account/Login");
            //ACT

            var response = await client.Post(formIndex: 2, defaults: new formValues()
            {
                {"Email", "Bobbie@kuifje.be"},
                {"Password", "P@ssw0rd!"}
            });
            Assert.Equal("/", client.AbsolutePath);
        }
        [Fact]
        public async Task Login_NotConfirmed()
        {
            var client = new ClientWrapper(Client);
            await client.Get("/Account/Login");
            //ACT

            var response = await client.Post(formIndex: 2, defaults: new formValues()
            {
                {"Email", "confirm@test.nl"},
                {"Password", "P@ssw0rd!"}
            });
            Assert.Contains("not confirmed", client.Html);
        }

        [Fact]
        public async Task Create_User_and_Login()
        {
            
            //Arrange
            var emailAddress = "new@test.nl";
            var client = new ClientWrapper(Client, TestMessageServices);
            //Act
            await client.Get("/User/Create");

            //Assert
            Assert.Equal("/Account/Login", client.AbsolutePath);
            //Account/Login
            await client.Post(2,new formValues {
                {"Email","thom@medella.nl"},
                { "Password","P@ssw0rd!"}
            });
            Assert.Equal("/User/Create", client.AbsolutePath);

            //ACT
            var response = await client.Post(formIndex: 1, defaults: new formValues()
            {
                {"UserName", emailAddress},
                {"Email", emailAddress},
                {"AccessFailedCount", "0"},
                {"TwoFactorEnabled", "true" }
            });
            //Assert
            Assert.Contains("success", client.Html);
            Assert.True(TestMessageServices.TestHtmlEmail.ContainsKey(emailAddress));

            //ACT
            await client.Click_on_Link_in_Email(emailAddress);
            //Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Contains("Enter your password to complete", client.Html);


            //ACT
            response = await client.Post(formIndex: 1, defaults: new formValues()
            {
                {"Password", "P@ssw0rd!"},
                {"ConfirmPassword", "P@ssw0rd!"},
                {"RemomberMe", "0"}
            });
            //Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(null, client.HtmlDocument.ErrorMsg());
            Assert.Equal("/Account/SendCode", client.AbsolutePath);


            //ACT
            response = await client.Post(formIndex: 1, defaults: new formValues());
            //Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(null, client.HtmlDocument.ErrorMsg());
            Assert.Equal("/Account/VerifyCode", client.AbsolutePath);

            //ACT  post verify code
            var code = client.getSecurityCode(emailAddress);
            response = await client.Post(formIndex: 1, defaults: new formValues()
            {
                { "Code",code}
            });
            //Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(null, client.HtmlDocument.ErrorMsg());
            Assert.Equal("/", client.AbsolutePath);

        }
    }
}

