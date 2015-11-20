using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DBC.Services;
using DBC.ViewModels.Account;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.AspNet.Mvc.FunctionalTests;
using System.Net.Http;
using DBC;
using Moq;
using Microsoft.AspNet.TestHost;
using DBC.Models.DB;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Localization;
using DBC.Controllers;
using Microsoft.Extensions.OptionsModel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace DBC.Controllers
{
    public class AccountControllerTest : IClassFixture<MvcTestFixture<DBC.StartupTest>>
    {
        public AccountControllerTest(MvcTestFixture<DBC.StartupTest> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }
        public TestServer Server { get; }
        public const string relPath = "../../src/DBC/";
        private string returnurl { get; set; } = "returnurl";

        [Fact]
        public void getLogin()
        {
            // Arrange
            var controller = MockAccountController(SignInResult.Success);
            // Act
            var viewResult = controller.Login(returnurl) as ViewResult;
            //assert
            Assert.Equal(returnurl, viewResult.ViewData["ReturnUrl"]);
        }
        [Fact]
        public async void InvalidLogin()
        {
            // Arrange
            var controller = MockAccountController(SignInResult.Failed);
            // Act
            var actionResult = await controller.Login(new LoginViewModel() { Email = "Failed", Password = "Failed", RememberMe = true }, "retrunurl");
            //assert
            Assert.Equal("Invalid login attempt.", controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).First());
        }
        [Fact]
        public async void NotAllowedLogin()
        {
            // Arrange
            var signInResult = SignInResult.NotAllowed;
            var controller = MockAccountController(signInResult);
            // Act
            var actionResult = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "retrunurl");
            //assert
            Assert.Equal("Email or phonenumber not confirmed", controller.ModelState.Values.Select(m => m.Errors[1].ErrorMessage).Last());
        }
        [Fact]
        public async void lockoutLogin()
        {
            // Arrange
            var signInResult = SignInResult.LockedOut;
            var controller = MockAccountController(signInResult);
            // Act
            var viewResult = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "retrunurl") as ViewResult;
            //assert
            Assert.Equal("Lockout", viewResult.ViewName);
        }
        [Fact]
        public async void TwoFactorLogin()
        {
            // Arrange
            var signInResult = SignInResult.TwoFactorRequired;
            var controller = MockAccountController(signInResult);
            // Act
            var redirectResult = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "retrunurl") as RedirectToActionResult;
            //assert
            Assert.Equal("SendCode", redirectResult.ActionName);
        }
        [Fact]
        public async void SuccessLoginLocalUrl()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(signInResult);
            // Act
            var redirectResult = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "localUrl") as RedirectResult;
            //assert
            Assert.Equal("localUrl", redirectResult.Url);
        }
        [Fact]
        public async void SuccessLoginRemoteUrl()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(signInResult);
            // Act
            var redirectResult = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "remoteUrl") as RedirectToActionResult;
            //assert
            Assert.Equal(nameof(HomeController),redirectResult.ControllerName+"Controller");
        }
        private static async Task TestMiddlewareFunc(Func<Task> next, HttpContext context)
        {
            await next();
            var emailTemplate = context.RequestServices.GetService<IEmailTemplate>();
            var body = await emailTemplate.RenderViewToString<ActivateEmail>(@"/Views/Email/ActivateEmail", new ActivateEmail { Emailaddress = "MyEmailAddress", Callback = "MyUrladdress" });
            await context.Response.WriteAsync(body);
        }
        private static AccountController MockAccountController(SignInResult signInResult)
        {
            var mockEmailSender = new Mock<IEmailSender>();
            var mockSmsSender = new Mock<ISmsSender>();
            var mockApplicationDbContext = new Mock<ApplicationDbContext>();
            var mockEmailTemplate = new Mock<IEmailTemplate>();
            var mockNullLoggerFactory = new Microsoft.Extensions.Logging.Testing.NullLoggerFactory();
            var MockUserManager = Microsoft.AspNet.Identity.Test.MockHelpers.MockUserManager<ApplicationUser>();
            var MockSignInManager = Microsoft.AspNet.Identity.Test.MockHelpers.MockSignInManager<ApplicationUser>();
            MockSignInManager.Setup(m => m.PasswordSignInAsync(nameof(signInResult), nameof(signInResult), true, false)).Returns(Task.FromResult<SignInResult>(signInResult));

            var mockLocalizer = new Mock<IStringLocalizer<AccountController>>();
            mockLocalizer.Setup(m => m[It.IsAny<string>()]).Returns<string>(x => new LocalizedString(x, x, false));

            var controller = new AccountController(MockUserManager.Object, MockSignInManager.Object, mockEmailSender.Object, mockSmsSender.Object, mockNullLoggerFactory, mockApplicationDbContext.Object, mockEmailTemplate.Object, mockLocalizer.Object);

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl("localUrl")).Returns(true);
            mockUrl.Setup(m => m.IsLocalUrl("remoteUrl")).Returns(false);
            controller.Url = mockUrl.Object;

            return controller;
        }
    }
}