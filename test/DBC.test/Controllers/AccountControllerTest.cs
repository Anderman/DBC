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
using System.Security.Claims;
using Microsoft.AspNet.Mvc.Rendering;
using static DBC.Test.MockHelpers;
namespace DBC.Controllers
{
    public class AccountControllerTest
    {
        private static string AnyString => It.IsAny<string>();
        private SendCodeViewModel sendCodeViewModel = new SendCodeViewModel()
        {
            Providers = new List<SelectListItem>() { new SelectListItem() { Selected = true, Value = "Email", Text = "Email Provider" } },
            RememberMe = true,
            ReturnUrl = "returnUrl",
            SelectedProvider = "Email"
        };
        private VerifyCodeViewModel verifyCodeViewModel = new VerifyCodeViewModel()
        {
            RememberMe = true,
            ReturnUrl = "localUrl",
            Code = "code",
            Provider = "Email",
            RememberBrowser = true
        };
        private ApplicationUser user = new ApplicationUser();
        private static Mock<IEmailSender> _mockEmailSender;
        private static Mock<IEmailTemplate> _mockEmailTemplate;
        private string returnurl { get; set; } = "returnurl";




        [Fact]
        public void getLogin()
        {
            // Arrange
            var controller = MockAccountController();
            // Act
            var result = controller.Login(returnurl) as ViewResult;
            //assert
            Assert.Equal(returnurl, result.ViewData["ReturnUrl"]);
        }
        [Fact]
        public async void Login_Invalid()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(SignInResult.Failed), MockUserManager(user));
            // Act
            var result = await controller.Login(new LoginViewModel() { Email = "Failed", Password = "Failed", RememberMe = true }, "retrunurl");
            //assert
            Assert.Equal("Invalid login attempt.", controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).First());
        }
        [Fact]
        public async void Login_NotAllowed()
        {
            // Arrange
            var signInResult = SignInResult.NotAllowed;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult), MockUserManager(user));
            // Act
            var result = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "retrunurl");
            //assert
            Assert.Equal("Email or phonenumber not confirmed", controller.ModelState.Values.Select(m => m.Errors[1].ErrorMessage).Last());
        }
        [Fact]
        public async void Login_lockout()
        {
            // Arrange
            var signInResult = SignInResult.LockedOut;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult), MockUserManager(user));
            // Act
            var result = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "retrunurl") as ViewResult;
            //assert
            Assert.Equal("Lockout", result?.ViewName);
        }
        [Fact]
        public async void Login_TwoFactor()
        {
            // Arrange
            var signInResult = SignInResult.TwoFactorRequired;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult), MockUserManager(user));
            // Act
            var result = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "retrunurl") as RedirectToActionResult;
            //assert
            Assert.Equal("SendCode", result.ActionName);
        }
        [Fact]
        public async void Login_Success_LocalUrl()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult), MockUserManager(user));
            // Act
            var result = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "localUrl") as RedirectResult;
            //assert
            Assert.Equal("localUrl", result.Url);
        }
        [Fact]
        public async void Login_Success_RemoteUrl()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult), MockUserManager(user));
            // Act
            var result = await controller.Login(new LoginViewModel() { Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true }, "remoteUrl") as RedirectToActionResult;
            //assert
            Assert.Equal(nameof(HomeController), result.ControllerName + "Controller");
        }




        [Fact]
        public void ExternalLogin()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var mockSignInManager = MockSignInManager<ApplicationUser>(signInResult);
            var controller = MockAccountController(mockSignInManager);
            // Act
            var result = controller.ExternalLogin("google", "localUrl") as ChallengeResult;
            //assert
            Assert.Equal("google", result.AuthenticationSchemes[0]);
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalWithoutError()
        {
            // Arrange
            var signInResult = SignInResult.Failed;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult, info: null));
            // Act
            var result = await controller.ExternalLoginCallback(null, null) as RedirectToActionResult;
            //assert
            Assert.Equal("Login", result.ActionName);
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalWithError()
        {
            // Arranget
            var signInResult = SignInResult.Failed;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(signInResult, info: null));
            // Act
            var result = await controller.ExternalLoginCallback(null, "error") as ViewResult;
            //assert
            Assert.Equal("Login", result?.ViewName);
            Assert.True(controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last().Contains("error"));
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalSucces_LocalNotConfirmed()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.Failed, DefaultInfo()), MockUserManager(user: user));
            // Act
            var result = await controller.ExternalLoginCallback("returnurl", null) as RedirectToActionResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("returnurl", result.RouteValues["returnurl"]);
            Assert.True(controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last().Contains("profile"));
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalSucces_LocalNotAllowed()
        {
            // Arrange
            var controller = MockAccountController(
                MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.NotAllowed, DefaultInfo()),
                MockUserManager(user: user));
            //var controller = MockAccountController(SignInResult.Failed, SignInResult.NotAllowed, nullInfo: false, EmailAddress: "RegisterUserEmail");
            // Act
            var result = await controller.ExternalLoginCallback("returnurl", null) as RedirectToActionResult;
            //assert
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("returnurl", result.RouteValues["returnurl"]);
            Assert.True(controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last().Contains("Confirmation"));
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalSucces_LocalLockedOut()
        {
            // Arrange
            var controller = MockAccountController(
                MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.LockedOut, DefaultInfo()),
                MockUserManager(user: user));
            //var controller = MockAccountController(SignInResult.Failed, SignInResult.LockedOut, nullInfo: false, EmailAddress: "RegisterUserEmail");
            // Act
            var result = await controller.ExternalLoginCallback("returnurl", null) as ViewResult;
            //assert
            Assert.Equal("Lockout", result?.ViewName);
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalSucces_LocalTwoFactorRequired()
        {
            // Arrange
            var controller = MockAccountController(
                MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.TwoFactorRequired, DefaultInfo()),
                MockUserManager(user: user));
            // Act
            var result = await controller.ExternalLoginCallback("returnurl", null) as RedirectToActionResult;
            //assert
            Assert.Equal("SendCode", result.ActionName);
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalSuccess_LocalSuccess_ExternalUrl()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(
                MockSignInManager<ApplicationUser>(SignInResult.Success, SignInResult.Success, DefaultInfo()));
            // Act
            var result = await controller.ExternalLoginCallback("externalUrl", null) as RedirectToActionResult;
            //assert
            Assert.Equal(nameof(HomeController), result.ControllerName + "Controller");
        }
        [Fact]
        public async void ExternalLoginCallback_ExternalSuccess_LocalSuccess_LocalUrl()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(
                MockSignInManager<ApplicationUser>(SignInResult.Success, SignInResult.Success, DefaultInfo()));
            // Act
            var result = await controller.ExternalLoginCallback("localUrl", null) as RedirectResult;
            //assert
            Assert.Equal("localUrl", result.Url);
        }




        [Fact]
        public async void ConfirmEmail_UserNotFound()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(), MockUserManager<ApplicationUser>());
            // Act
            var result = await controller.ConfirmEmail("userIdInvalid", "codeInvalid") as ViewResult;
            //assert
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public async void ConfirmEmail_UserFound_CodeInvalid()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(), MockUserManager(user, IdentityResult.Failed()));
            // Act
            var result = await controller.ConfirmEmail("userId", "code") as ViewResult;
            //assert
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public async void ConfirmEmail_UserFound_CodeValid()
        {
            // Arrange
            var signInResult = SignInResult.Success;
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(), MockUserManager(user, IdentityResult.Success));
            // Act

            var result = await controller.ConfirmEmail("userId", "code") as ViewResult;
            //assert
            Assert.Equal("ConfirmEmail", result?.ViewName);
        }

        [Fact]
        public void ForgotPassword()
        {
            // Arrange
            var controller = MockAccountController();
            // Act
            var result = controller.ForgotPassword() as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("ForgotPassword", result?.ViewName ?? "ForgotPassword");
        }
        [Fact]
        public async void ForgotPassword_UserNotFound()
        {
            // Arrange
            var controller = MockAccountController(null, MockUserManager<ApplicationUser>());
            // Act

            var result = await controller.ForgotPassword(new ForgotPasswordViewModel() { Email = "Emailaddress@" }) as ViewResult;
            //assert
            Assert.Equal("ForgotPasswordConfirmation", result?.ViewName);
        }
        [Fact]
        public async void ForgotPassword_User_NotConfirmed()
        {
            // Arrange
            var controller = MockAccountController(null, MockUserManager(user, isEmailConfirmedAsync: false));
            // Act

            var result = await controller.ForgotPassword(new ForgotPasswordViewModel() { Email = "Emailaddress@" }) as ViewResult;
            //assert
            Assert.Equal("ForgotPasswordConfirmation", result?.ViewName);
        }
        [Fact]
        public async void ForgotPassword_User_Confirmed()
        {
            // Arrange
            var controller = MockAccountController(null, MockUserManager(user, isEmailConfirmedAsync: true));
            _mockEmailTemplate.Setup(m => m.RenderViewToString<ActivateEmail>(AnyString, It.IsAny<ActivateEmail>())).Returns((string x, ActivateEmail y) => Task.FromResult(x + y.Callback + y.Emailaddress));
            _mockEmailSender.Setup(m => m.SendEmailAsync("EmailAddress@", "Reset password", AnyString)).Verifiable();
            // Act

            var result = await controller.ForgotPassword(new ForgotPasswordViewModel() { Email = "Emailaddress@" }) as ViewResult;
            //assert
            Assert.Equal("ForgotPasswordConfirmation", result?.ViewName);
            _mockEmailTemplate.Verify(m => m.RenderViewToString(AnyString, It.IsAny<object>()), Times.Once);
            _mockEmailSender.Verify(m => m.SendEmailAsync(AnyString, AnyString, AnyString), Times.Once);
        }

        [Fact]
        public void ForgotPasswordConfirmation()
        {
            // Arrange
            var controller = MockAccountController();
            // Act
            var result = controller.ForgotPasswordConfirmation() as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("ForgotPasswordConfirmation", result?.ViewName ?? "ForgotPasswordConfirmation");
        }

        [Fact]
        public void ResetPassword_WithoutCode()
        {
            // Arrange
            var controller = MockAccountController();
            // Act
            var result = controller.ResetPassword() as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public void ResetPassword_withCode()
        {
            // Arrange
            var controller = MockAccountController();
            // Act
            var result = controller.ResetPassword("code") as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("ResetPassword", result?.ViewName ?? "ResetPassword");
        }

        [Fact]
        public async void ResetPassword_with_Registered_User()
        {
            // Arrange
            var controller = MockAccountController(null, MockUserManager<ApplicationUser>());
            // Act
            var result = await controller.ResetPassword(new ResetPasswordViewModel()) as RedirectToActionResult;
            //assert
            Assert.Equal(nameof(AccountController.ResetPasswordConfirmation), result?.ActionName);
        }
        [Fact]
        public async void ResetPassword_with_Registered_User_And_Invalid_Code()
        {
            // Arrange
            var controller = MockAccountController(null, MockUserManager(user, IdentityResult.Failed(new IdentityError() { Code = "1", Description = "Error" })));
            // Act

            var result = await controller.ResetPassword(new ResetPasswordViewModel() { Code = "token", Password = "password", Email = "EmailAddress@" }) as ViewResult;
            //assert
            Assert.NotNull(result);

            Assert.Equal("ResetPassword", result?.ViewName ?? "ResetPassword");
            Assert.Equal("Error", controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).First());
        }
        [Fact]
        public async void ResetPassword_with_registered_user_and_valid_code()
        {
            // Arrange
            var controller = MockAccountController(null, MockUserManager(user, IdentityResult.Success));
            // Act
            var result = await controller.ResetPassword(new ResetPasswordViewModel() { Code = "token", Password = "password", Email = "EmailAddress@" }) as RedirectToActionResult;

            //assert
            Assert.Equal(nameof(AccountController.ResetPasswordConfirmation), result?.ActionName);
            //_mockUserManager.Verify(m => m.ResetPasswordAsync(user, AnyString, AnyString), Times.Once);
        }



        [Fact]
        public async void SendCode_AnonymousUser()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(), MockUserManager<ApplicationUser>());
            // Act
            var result = await controller.SendCode("returnurl", rememberMe: true) as ViewResult;

            //assert
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public async void SendCode_RegisteredUser()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager(user), MockUserManager(user));
            // Act
            var result = await controller.SendCode("returnurl", rememberMe: true) as ViewResult;

            //assert
            Assert.NotNull(result);
            Assert.NotNull(controller.ViewData.Model);
            Assert.Equal(nameof(AccountController.SendCode), result?.ViewName ?? "SendCode");
        }
        [Fact]
        public async void SendCode_Post_AnonymousUser()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(), MockUserManager<ApplicationUser>());
            // Act
            var result = await controller.SendCode(sendCodeViewModel) as ViewResult;

            //assert
            Assert.NotNull(result);
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public async void SendCode_Post_RegisteredUser_nullToken()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager(user), MockUserManager(user));
            // Act
            var result = await controller.SendCode(sendCodeViewModel) as ViewResult;

            //assert
            Assert.NotNull(result);
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public async void SendCode_Post_RegisteredUser_RealToken()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager(user), MockUserManager(user, "realToken"));
            // Act
            var result = await controller.SendCode(sendCodeViewModel) as RedirectToActionResult;

            //assert
            _mockEmailSender.Verify(m => m.SendEmailAsync("EmailAddress@", "Security Code", It.IsRegex(".*realToken.*")), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(nameof(AccountController.VerifyCode), result?.ActionName);
        }

        [Fact]
        public async void VerifyCode_AnonymousUser()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(), MockUserManager<ApplicationUser>());
            // Act
            var result = await controller.VerifyCode("Email", true, "returnurl") as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("Error", result?.ViewName);
        }
        [Fact]
        public async void VerifyCode_RegisteredUser()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager(user), MockUserManager(user));
            // Act
            var result = await controller.VerifyCode("Email", true, "returnurl") as ViewResult;

            //assert
            Assert.NotNull(result);
            Assert.NotNull(controller.ViewData.Model);
            Assert.Equal(nameof(AccountController.VerifyCode), result?.ViewName ?? nameof(AccountController.VerifyCode));
        }
        [Fact]
        public async void VerifyCode_Post_User()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(SignInResult.Failed), MockUserManager(user));
            // Act
            var result = await controller.VerifyCode(verifyCodeViewModel) as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal(nameof(AccountController.VerifyCode), result?.ViewName ?? nameof(AccountController.VerifyCode));
        }
        [Fact]
        public async void VerifyCode_Post_User_LockedOut()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(SignInResult.LockedOut), MockUserManager(user));
            // Act
            var result = await controller.VerifyCode(verifyCodeViewModel) as ViewResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("Lockout", result?.ViewName);
        }
        [Fact]
        public async void VerifyCode_Post_User_Success()
        {
            // Arrange
            var controller = MockAccountController(MockSignInManager<ApplicationUser>(SignInResult.Success), MockUserManager(user));
            // Act
            var result = await controller.VerifyCode(verifyCodeViewModel) as RedirectResult;
            //assert
            Assert.NotNull(result);
            Assert.Equal("localUrl", result?.Url);
        }













        private static async Task TestMiddlewareFunc(Func<Task> next, HttpContext context)
        {
            await next();
            var emailTemplate = context.RequestServices.GetService<IEmailTemplate>();
            var body = await emailTemplate.RenderViewToString<ActivateEmail>(@"/Views/Email/ActivateEmail", new ActivateEmail { Emailaddress = "MyEmailAddress", Callback = "MyUrladdress" });
            await context.Response.WriteAsync(body);
        }


        //Mock AccountController
        private static AccountController MockAccountController()
        {
            return MockAccountController(null);
        }
        private static AccountController MockAccountController(SignInManager<ApplicationUser> mockSignInManager)
        {
            return MockAccountController(mockSignInManager, null);
        }
        private static AccountController MockAccountController(SignInManager<ApplicationUser> mockSignInManager, UserManager<ApplicationUser> mockUserManager)
        {
            _mockEmailSender = new Mock<IEmailSender>();
            var mockSmsSender = new Mock<ISmsSender>();
            var mockApplicationDbContext = new Mock<ApplicationDbContext>();
            _mockEmailTemplate = new Mock<IEmailTemplate>();
            var mockNullLoggerFactory = new Microsoft.Extensions.Logging.Testing.NullLoggerFactory();


            var mockLocalizer = new Mock<IStringLocalizer<AccountController>>();
            mockLocalizer.Setup(m => m[It.IsAny<string>()])
                .Returns<string>(x => new LocalizedString(x, x, false));
            mockLocalizer.Setup(m => m[It.IsAny<string>(), new object[] { It.IsAny<object>() }])
                .Returns<string, object[]>((x, y) => new LocalizedString(x, string.Format(x, y), false));

            var controller = new AccountController(mockUserManager, mockSignInManager, _mockEmailSender.Object, mockSmsSender.Object, mockNullLoggerFactory, mockApplicationDbContext.Object, _mockEmailTemplate.Object, mockLocalizer.Object);

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl("localUrl")).Returns(true);
            mockUrl.Setup(m => m.IsLocalUrl("remoteUrl")).Returns(false);
            //mockUrl.Setup(m => m.Action(AnyString,AnyString,It.IsAny<object>(),AnyString)).Returns((string action,string contr,object o,string scheme)=>action+contr);

            controller.Url = mockUrl.Object;

            //var httpRequest = new Mock<HttpRequest>();

            //var mockHttpContext = new Mock<HttpContext>();
            //mockHttpContext.Setup(m => m.Request).Returns(httpRequest.Object);
            //controller.HttpContext = mockHttpContext.Object;

            return controller;
        }




    }
}