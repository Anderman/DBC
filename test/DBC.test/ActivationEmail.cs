using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DBC.Services;
using DBC.ViewModels.Account;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Xunit;
using Microsoft.AspNet.Mvc.FunctionalTests;
using System.Net.Http;
using DBC;
using Moq;
using Microsoft.AspNet.TestHost;
using DBC.Models.DB;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.Localization;
using DBC.Controllers;
using Microsoft.Framework.OptionsModel;
using System.Collections.Generic;
using Microsoft.Framework.Logging;
using Microsoft.AspNet.Mvc;

namespace DBC.Controllers
{
    public class AccountControllerTest : IClassFixture<MvcTestFixture<DBC.StartupTest>>
    {
        public AccountControllerTest(MvcTestFixture<DBC.StartupTest> fixture)
        {
            Client = fixture.Client;
            Server = fixture.Server; ;
        }

        public HttpClient Client { get; }
        public TestServer Server { get; }
        public const string relPath = "../../src/DBC/";
        private string returnurl { get; set; } = "returnurl";

        [Fact]
        public async void Login()
        {
            // Arrange
            var MockUserManager = Microsoft.AspNet.Identity.Test.MockHelpers.MockUserManager<ApplicationUser>();
            var MockSignInManager = Microsoft.AspNet.Identity.Test.MockHelpers.MockSignInManager<ApplicationUser>();

            var emailSender = new Mock<IEmailSender>();
            var smsSender = new Mock<ISmsSender>();
            var applicationDbContext = new Mock<ApplicationDbContext>();
            var emailTemplate = new Mock<IEmailTemplate>();
            var localizer = new Mock<IStringLocalizer<AccountController>>();
            MockSignInManager.Setup(m => m.PasswordSignInAsync("Success", "Success", true, false)).Returns(Task.FromResult<SignInResult>(SignInResult.Success));
            MockSignInManager.Setup(m => m.PasswordSignInAsync("Failed", "Failed", true, false)).Returns(Task.FromResult<SignInResult>(SignInResult.Failed));
            MockSignInManager.Setup(m => m.PasswordSignInAsync("LockedOut", "LockedOut", true, false)).Returns(Task.FromResult<SignInResult>(SignInResult.LockedOut));
            MockSignInManager.Setup(m => m.PasswordSignInAsync("NotAllowed", "NotAllowed", true, false)).Returns(Task.FromResult<SignInResult>(SignInResult.NotAllowed));
            MockSignInManager.Setup(m => m.PasswordSignInAsync("TwoFactorRequired", "TwoFactorRequired", true, false)).Returns(Task.FromResult<SignInResult>(SignInResult.TwoFactorRequired));

            // Act
            var controller = new AccountController(MockUserManager.Object, MockSignInManager.Object, emailSender.Object, smsSender.Object, applicationDbContext.Object, emailTemplate.Object, localizer.Object);
            var result = controller.Login(returnurl) as ViewResult;
            Assert.Equal(returnurl, result.ViewData["ReturnUrl"]);


            Debugger.Launch();
            var y = await controller.Login(new LoginViewModel() { Email = "Failed", Password = "Failed", RememberMe = true }, "retrunurl");
            //MockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), true, false)).Returns(Task.FromResult<SignInResult>(new SignInResult() { Succeeded = true }));
            //MockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), true, false)).Returns(Task.FromResult<SignInResult>(new SignInResult() { Succeeded = true }));
            Assert.Equal(1, controller.ModelState.Values.Count);

            //var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), relPath, "wwwroot/");
            //var response = await Client.GetAsync("http://localhost/Account/Login");
            //var responseBody = await response.Content.ReadAsStringAsync();
            ////Assert
            //Assert.Contains("Activeer mijn account", responseBody);// Added email template to view
            //Assert.Contains("MyEmailAddress", responseBody);
            //Assert.Contains("MyUrladdress", responseBody);
        }
        private static async Task TestMiddlewareFunc(Func<Task> next, HttpContext context)
        {
            await next();
            var emailTemplate = context.RequestServices.GetRequiredService<IEmailTemplate>();
            var body = await emailTemplate.RenderViewToString<ActivateEmail>(@"/Views/Email/ActivateEmail", new ActivateEmail { Emailaddress = "MyEmailAddress", Callback = "MyUrladdress" });
            await context.Response.WriteAsync(body);
        }
    }
}