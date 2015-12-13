using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DBC.test.HtmlHelper;
using DBC.test.TestApplication;
using Xunit;

namespace DBC.test.Controllers
{
    public class ManageControllerTest : IClassFixture<TestApplicationFixture<StartupTest>>
    {

        public StartupTest Server { get; }
        public HttpClient Client { get; }

        public ManageControllerTest(TestApplicationFixture<StartupTest> fixture)
        {
            Server = (StartupTest)fixture.Server;
            Client = fixture.Client;
        }

        [Fact]
        public async Task Index()
        {
            string emailAddress = "Bobbie@kuifje.be";
            string password = "P@ssw0rd!";
            //Arrange
            var client = new ClientWrapper(Client);
            client.Login(emailAddress, password);
            //Act
            await client.Get("/Manage/Index");
            //Assert
            Assert.Equal(null, client.ValidateResponse("/Manage/Index"));
            Assert.Equal("Manage/ChangePassword", client.HasLink("Manage/ChangePassword"));

            //Act
            await client.Get("Manage/ChangePassword");
            //Assert
            Assert.Equal(null, client.ValidateResponse("/Manage/ChangePassword"));
        }
    }
}
