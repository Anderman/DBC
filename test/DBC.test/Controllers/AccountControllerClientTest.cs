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

namespace DBC.test.Controllers
{
    public class AccountControllerClientTest : IClassFixture<TestApplicationFixture<DBC.StartupTest>>
    {
        public StartupTest MyApp { get; }
        public TestMessageServices TestMessageServices { get; }

        public HttpClient Client { get; }
        public TestServer Server { get; }
        public const string relPath = "../../src/DBC/";
        public AccountControllerClientTest(TestApplicationFixture<StartupTest> fixture)
        {
            MyApp = (StartupTest)fixture.MyApp;
            Client = fixture.Client;
            TestMessageServices = MyApp.ServiceProvider.GetRequiredService<IEmailSender>() as TestMessageServices;
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
            var response = await client.Post(formIndex:2, defaults: new formValues()
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
            var emailAddress = "new@test.nl";
            //Arrange
            var client = new ClientWrapper(Client,TestMessageServices);

            //Act
            await client.Get("/User/Create");
            //Assert
            Assert.Contains("Create User", client.Html);

            //ACT
            var response = await client.Post(formIndex: 1, defaults: new formValues()
            {
                {"UserName", emailAddress},
                {"Email", emailAddress},
                {"AccessFailedCount", "0"}
            });
            //Assert
            Assert.Contains("success", client.Html); 
            Assert.True(TestMessageServices.TestHtmlEmail.ContainsKey("new@test.nl"));

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
            Assert.Equal(null, client.Doc.ErrorMsg());
            Assert.Equal("/", client.AbsolutePath);
        }
    }
}

