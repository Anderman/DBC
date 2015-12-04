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
            await client.Post("/Account/Login", client.Form(2), new formValues()
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
            var response = await client.Post("/Account/Login", client.Form(2), new formValues()
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

            var response = await client.Post("/Account/Login", client.Form(2), new formValues()
            {
                {"Email", "confirm@test.nl"},
                {"Password", "P@ssw0rd!"}
            });
            Assert.Contains("not confirmed", client.Html);
        }

        [Fact]
        public async Task Create_User()
        {
            var emailAddress = "new@test.nl";
            //Arrange
            var client = new ClientWrapper(Client,TestMessageServices);

            //Act
            await client.Get("/User/Create");
            //Assert
            Assert.Contains("Create User", client.Html);

            //ACT
            var response = await client.Post("/User/Create", client.Form(1), new formValues()
            {
                {"UserName", emailAddress},
                {"Email", emailAddress},
                {"AccessFailedCount", "0"}
            });
            //Assert
            Assert.Contains("success", client.Html); //succesfull send email message
            Assert.True(TestMessageServices.TestHtmlEmail.ContainsKey("new@test.nl"));

            //ACT
            await client.Click_on_Link_in_Email(emailAddress);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Contains("Enter your password to complete", client.Html);
        }


        //var response = await Client.GetAsync("/Account/Login");
        //var responseBody = await response.Content.ReadAsStringAsync();
        //Debugger.Launch();
        //Assert.Contains("Login", await response.Content.ReadAsStringAsync());

        //var request = new HttpRequestMessage(HttpMethod.Post, "/Account/Login");
        //request.Headers.Add("Cookie", cookieToken.Key + "=" + cookieToken.Value);

        //var nameValueCollection = new List<KeyValuePair<string, string>>
        //{
        //    new KeyValuePair<string,string>("__RequestVerificationToken", formToken),
        //};

    }

}

