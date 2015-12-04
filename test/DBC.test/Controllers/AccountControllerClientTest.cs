using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNet.TestHost;
using Xunit;
using DBC.test.HtmlHelpers;
using DBC.test.HtmlHelper;
using DBC.test.TestApplication;

namespace DBC.test.Controllers
{
    public class AccountControllerClientTest : IClassFixture<MvcTestFixture<DBC.StartupTest>>
    {
        public HttpClient Client { get; }
        public TestServer Server { get; }
        public const string relPath = "../../src/DBC/";
        public AccountControllerClientTest(MvcTestFixture<StartupTest> fixture)
        {

            Client = fixture.Client;
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
            var client = new ClientWrapper(Client);
            await client.Get("/User/Create");
            //ACT

            var response = await client.Post("/User/Create", client.Form(1), new formValues()
            {
                {"UserName", "new@test.nl"},
                {"Email", "new@test.nl"},
                { "AccessFailedCount","0"}
            });
            Assert.Contains("success", client.Html); //succesfull send email message
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

