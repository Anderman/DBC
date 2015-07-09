using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DBC.Models.View;
using DBC.Services;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace DBC.test
{
    public class ActivationEmail
    {

        [Fact]
        public async void ActivateEmailTest()
        {
            // Arrange
            //       var a = TestServer.Create(new DBC.Startup().Configure, _configureServices);
            var applicationWebSiteName = nameof(DBC);
            var server = TestHelper.SetupServer(applicationWebSiteName, TestMiddlewareFunc);
            
            var client = server.CreateClient();

            // Act
            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(),"../src/DBC/wwwroot/");
            var response = await client.GetAsync("http://localhost/Account/Empty");
            var responseBody = await response.Content.ReadAsStringAsync();
#if(DEBUG)
            System.IO.File.WriteAllText($@"{wwwroot}\test.html", responseBody.Replace("~", wwwroot));
#endif
            //Assert
            Assert.Contains("Activeer mijn account", responseBody);// Added email template to view
            Assert.Contains("MyEmailAddress", responseBody);
            Assert.Contains("MyUrladdress", responseBody);
        }
        private static async Task TestMiddlewareFunc(Func<Task> next, HttpContext context)
        {
            await next();
            var emailTemplate = context.RequestServices.GetRequiredService<IEmailTemplate>();
            var body = await emailTemplate.RenderViewToString("Email", "ActivateEmail",new ActivateEmail {Emailaddress = "MyEmailAddress", Callback = "MyUrladdress"});
            await context.Response.WriteAsync(body);
        }
    }
}