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
        public async Task Login()
        {


            //Arrange
            //
            //ACT
            //
            //Assert

            var c = new ClientWrapper(Client);
            var y = await c.Get("/Account/Login", 2);
            var z = await c.Post("/Account/Login", 1, new formValues() {
                { "Email","Bobbie@kuifje.be" },
                { "Password","@Password!" }
            });

            

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
}
