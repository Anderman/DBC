using System.Collections.Generic;
using System.Linq;
using DBC.Controllers;
using DBC.Models.DB;
using DBC.Services;
using DBC.Test;
using DBC.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using Xunit;

namespace DBC.test.Controllers
{
    public class AccountControllerTest
    {
        private static readonly SendCodeViewModel SendCodeViewModel = new SendCodeViewModel
        {
            Providers = new List<SelectListItem> {new SelectListItem {Selected = true, Value = "Email", Text = "Email Provider"}},
            RememberMe = true,
            ReturnUrl = "returnUrl",
            SelectedProvider = "Email"
        };

        private static readonly VerifyCodeViewModel VerifyCodeViewModel = new VerifyCodeViewModel
        {
            RememberMe = true,
            ReturnUrl = "localUrl",
            Code = "code",
            Provider = "Email",
            RememberBrowser = true
        };

        private static readonly ApplicationUser User = new ApplicationUser();
        private static readonly string Returnurl = "returnurl";
        private static string AnyString => It.IsAny<string>();
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
            var mockEmailSender = new Mock<IEmailSender>();
            var mockSmsSender = new Mock<ISmsSender>();
            var mockApplicationDbContext = new Mock<ApplicationDbContext>();
            var mockEmailTemplate = new Mock<IEmailTemplate>();
            var mockNullLoggerFactory = new NullLoggerFactory();


            var mockLocalizer = new Mock<IStringLocalizer<AccountController>>();
            mockLocalizer.Setup(m => m[It.IsAny<string>()])
                .Returns<string>(x => new LocalizedString(x, x, false));
            mockLocalizer.Setup(m => m[It.IsAny<string>(), It.IsAny<object>()])
                .Returns<string, object[]>((x, y) => new LocalizedString(x, string.Format(x, y), false));

            var controller = new AccountController(mockUserManager, mockSignInManager, mockEmailSender.Object, mockSmsSender.Object, mockNullLoggerFactory, mockApplicationDbContext.Object, mockEmailTemplate.Object, mockLocalizer.Object);

            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(m => m.IsLocalUrl("localUrl")).Returns(true);
            mockUrl.Setup(m => m.IsLocalUrl("remoteUrl")).Returns(false);

            controller.Url = mockUrl.Object;

            return controller;
        }

        public class Login
        {
            [Fact]
            public void Get()
            {
                // Arrange
                var controller = MockAccountController();
                // Act
                var result = controller.Login(Returnurl) as ViewResult;
                //assert
                Assert.Equal(Returnurl, result.ViewData["ReturnUrl"]);
            }

            [Fact]
            public async void Invalid()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Failed), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.Login(new LoginViewModel {Email = "Failed", Password = "Failed", RememberMe = true}, "retrunurl");
                //assert
                Assert.Equal("Invalid login attempt.", controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).First());
            }

            [Fact]
            public async void NotAllowed()
            {
                // Arrange
                var signInResult = SignInResult.NotAllowed;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.Login(new LoginViewModel {Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true}, "retrunurl");
                //assert
                Assert.Equal("Email or phonenumber not confirmed", controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last());
            }

            [Fact]
            public async void Lockout()
            {
                // Arrange
                var signInResult = SignInResult.LockedOut;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.Login(new LoginViewModel {Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true}, "retrunurl") as ViewResult;
                //assert
                Assert.Equal("Lockout", result?.ViewName);
            }

            [Fact]
            public async void TwoFactor()
            {
                // Arrange
                var signInResult = SignInResult.TwoFactorRequired;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.Login(new LoginViewModel {Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true}, "retrunurl") as RedirectToActionResult;
                //assert
                Assert.Equal("SendCode", result.ActionName);
            }

            [Fact]
            public async void Success_LocalUrl()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.Login(new LoginViewModel {Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true}, "localUrl") as RedirectResult;
                //assert
                Assert.Equal("localUrl", result.Url);
            }

            [Fact]
            public async void Success_RemoteUrl()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.Login(new LoginViewModel {Email = nameof(signInResult), Password = nameof(signInResult), RememberMe = true}, "remoteUrl") as RedirectToActionResult;
                //assert
                Assert.Equal(nameof(HomeController), result.ControllerName + "Controller");
            }
        }

        public class ExternalLogin
        {
            [Fact]
            public void Get()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var mockSignInManager = MockHelpers.MockSignInManager<ApplicationUser>(signInResult);
                var controller = MockAccountController(mockSignInManager);
                // Act
                var result = controller.ExternalLogin("google", "localUrl") as ChallengeResult;
                //assert
                Assert.Equal("google", result.AuthenticationSchemes[0]);
            }

            [Fact]
            public async void Callback_ExternalWithoutError()
            {
                // Arrange
                var signInResult = SignInResult.Failed;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult, null));
                // Act
                var result = await controller.ExternalLoginCallback(null, null) as RedirectToActionResult;
                //assert
                Assert.Equal("Login", result.ActionName);
            }

            [Fact]
            public async void Callback_ExternalWithError()
            {
                // Arranget
                var signInResult = SignInResult.Failed;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(signInResult, null));
                // Act
                var result = await controller.ExternalLoginCallback(null, "error") as ViewResult;
                //assert
                Assert.Equal("Login", result?.ViewName);
                Assert.True(controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last().Contains("error"));
            }

            [Fact]
            public async void Callback_ExternalSucces_LocalNotConfirmed()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.Failed, MockHelpers.DefaultInfo()), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.ExternalLoginCallback("returnurl", null) as RedirectToActionResult;
                //assert
                Assert.NotNull(result);
                Assert.Equal("Login", result.ActionName);
                Assert.Equal("returnurl", result.RouteValues["returnurl"]);
                Assert.True(controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last().Contains("profile"));
            }

            [Fact]
            public async void Callback_ExternalSucces_LocalNotAllowed()
            {
                // Arrange
                var controller = MockAccountController(
                    MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.NotAllowed, MockHelpers.DefaultInfo()),
                    MockHelpers.MockUserManager(User));
                //var controller = MockAccountController(SignInResult.Failed, SignInResult.NotAllowed, nullInfo: false, EmailAddress: "RegisterUserEmail");
                // Act
                var result = await controller.ExternalLoginCallback("returnurl", null) as RedirectToActionResult;
                //assert
                Assert.Equal("Login", result.ActionName);
                Assert.Equal("returnurl", result.RouteValues["returnurl"]);
                Assert.True(controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).Last().Contains("Confirmation"));
            }

            [Fact]
            public async void Callback_ExternalSucces_LocalLockedOut()
            {
                // Arrange
                var controller = MockAccountController(
                    MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.LockedOut, MockHelpers.DefaultInfo()),
                    MockHelpers.MockUserManager(User));
                //var controller = MockAccountController(SignInResult.Failed, SignInResult.LockedOut, nullInfo: false, EmailAddress: "RegisterUserEmail");
                // Act
                var result = await controller.ExternalLoginCallback("returnurl", null) as ViewResult;
                //assert
                Assert.Equal("Lockout", result?.ViewName);
            }

            [Fact]
            public async void Callback_ExternalSucces_LocalTwoFactorRequired()
            {
                // Arrange
                var controller = MockAccountController(
                    MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Failed, SignInResult.TwoFactorRequired, MockHelpers.DefaultInfo()),
                    MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.ExternalLoginCallback("returnurl", null) as RedirectToActionResult;
                //assert
                Assert.Equal("SendCode", result.ActionName);
            }

            [Fact]
            public async void Callback_ExternalSuccess_LocalSuccess_ExternalUrl()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(
                    MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Success, SignInResult.Success, MockHelpers.DefaultInfo()));
                // Act
                var result = await controller.ExternalLoginCallback("externalUrl", null) as RedirectToActionResult;
                //assert
                Assert.Equal(nameof(HomeController), result.ControllerName + "Controller");
            }

            [Fact]
            public async void Callback_ExternalSuccess_LocalSuccess_LocalUrl()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(
                    MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Success, SignInResult.Success, MockHelpers.DefaultInfo()));
                // Act
                var result = await controller.ExternalLoginCallback("localUrl", null) as RedirectResult;
                //assert
                Assert.Equal("localUrl", result.Url);
            }
        }

        public class ConfirmEmail
        {
            [Fact]
            public async void UserNotFound()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(), MockHelpers.MockUserManager<ApplicationUser>());
                // Act
                var result = await controller.ConfirmEmail("userIdInvalid", "codeInvalid") as ViewResult;
                //assert
                Assert.Equal("Error", result?.ViewName);
            }

            [Fact]
            public async void UserFound_CodeInvalid()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(), MockHelpers.MockUserManager(User, IdentityResult.Failed()));
                // Act
                var result = await controller.ConfirmEmail("userId", "code") as ViewResult;
                //assert
                Assert.Equal("Error", result?.ViewName);
            }

            [Fact]
            public async void UserFound_CodeValid()
            {
                // Arrange
                var signInResult = SignInResult.Success;
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(), MockHelpers.MockUserManager(User, IdentityResult.Success));
                // Act

                var result = await controller.ConfirmEmail("userId", "code") as ViewResult;
                //assert
                Assert.Equal("ConfirmEmail", result?.ViewName);
            }
        }

        public class ForgotPassword
        {
            [Fact]
            public void Get()
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
            public async void User_NotFound()
            {
                // Arrange
                var controller = MockAccountController(null, MockHelpers.MockUserManager<ApplicationUser>());
                // Act

                var result = await controller.ForgotPassword(new ForgotPasswordViewModel {Email = "Emailaddress@"}) as ViewResult;
                //assert
                Assert.Equal("ForgotPasswordConfirmation", result?.ViewName);
            }

            [Fact]
            public async void User_NotConfirmed()
            {
                // Arrange
                var controller = MockAccountController(null, MockHelpers.MockUserManager(User, false));
                // Act

                var result = await controller.ForgotPassword(new ForgotPasswordViewModel {Email = "Emailaddress@"}) as ViewResult;
                //assert
                Assert.Equal("ForgotPasswordConfirmation", result?.ViewName);
            }

            [Fact]
            public async void User_Confirmed()
            {
                // Arrange
                //_mockEmailTemplate.Setup(m => m.RenderViewToString<ActivateEmail>(AnyString, It.IsAny<ActivateEmail>())).Returns((string x, ActivateEmail y) => Task.FromResult(x + y.Callback + y.Emailaddress));
                //_mockEmailSender.Setup(m => m.SendEmailAsync("EmailAddress@", "Reset password", AnyString)).Verifiable();
                var controller = MockAccountController(null, MockHelpers.MockUserManager(User, true));
                // Act

                var result = await controller.ForgotPassword(new ForgotPasswordViewModel {Email = "Emailaddress@"}) as ViewResult;
                //assert
                Assert.Equal("ForgotPasswordConfirmation", result?.ViewName);
            }
        }

        public class ForgotPasswordConfirmation
        {
            [Fact]
            public void Get()
            {
                // Arrange
                var controller = MockAccountController();
                // Act
                var result = controller.ForgotPasswordConfirmation() as ViewResult;
                //assert
                Assert.NotNull(result);
                Assert.Equal("ForgotPasswordConfirmation", result?.ViewName ?? "ForgotPasswordConfirmation");
            }
        }

        public class ResetPassword
        {
            [Fact]
            public void WithoutCode()
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
            public void WithCode()
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
            public async void With_Registered_User()
            {
                // Arrange
                var controller = MockAccountController(null, MockHelpers.MockUserManager<ApplicationUser>());
                // Act
                var result = await controller.ResetPassword(new ResetPasswordViewModel()) as RedirectToActionResult;
                //assert
                Assert.Equal(nameof(AccountController.ResetPasswordConfirmation), result?.ActionName);
            }

            [Fact]
            public async void With_Registered_User_And_Invalid_Code()
            {
                // Arrange
                var controller = MockAccountController(null, MockHelpers.MockUserManager(User, IdentityResult.Failed(new IdentityError {Code = "1", Description = "Error"})));
                // Act

                var result = await controller.ResetPassword(new ResetPasswordViewModel {Code = "token", Password = "password", Email = "EmailAddress@"}) as ViewResult;
                //assert
                Assert.NotNull(result);

                Assert.Equal("ResetPassword", result?.ViewName ?? "ResetPassword");
                Assert.Equal("Error", controller.ModelState.Values.Select(m => m.Errors[0].ErrorMessage).First());
            }

            [Fact]
            public async void With_registered_user_and_valid_code()
            {
                // Arrange
                var controller = MockAccountController(null, MockHelpers.MockUserManager(User, IdentityResult.Success));
                // Act
                var result = await controller.ResetPassword(new ResetPasswordViewModel {Code = "token", Password = "password", Email = "EmailAddress@"}) as RedirectToActionResult;

                //assert
                Assert.Equal(nameof(AccountController.ResetPasswordConfirmation), result?.ActionName);
                //_mockUserManager.Verify(m => m.ResetPasswordAsync(user, AnyString, AnyString), Times.Once);
            }
        }

        public class SendCode
        {
            [Fact]
            public async void AnonymousUser()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(), MockHelpers.MockUserManager<ApplicationUser>());
                // Act
                var result = await controller.SendCode("returnurl", true) as ViewResult;

                //assert
                Assert.Equal("Error", result?.ViewName);
            }

            [Fact]
            public async void RegisteredUser()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager(User), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.SendCode("returnurl", true) as ViewResult;

                //assert
                Assert.NotNull(result);
                Assert.NotNull(controller.ViewData.Model);
                Assert.Equal(nameof(AccountController.SendCode), result?.ViewName ?? "SendCode");
            }

            [Fact]
            public async void Post_AnonymousUser()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(), MockHelpers.MockUserManager<ApplicationUser>());
                // Act
                var result = await controller.SendCode(SendCodeViewModel) as ViewResult;

                //assert
                Assert.NotNull(result);
                Assert.Equal("Error", result?.ViewName);
            }

            [Fact]
            public async void Post_RegisteredUser_nullToken()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager(User), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.SendCode(SendCodeViewModel) as ViewResult;

                //assert
                Assert.NotNull(result);
                Assert.Equal("Error", result?.ViewName);
            }

            [Fact]
            public async void Post_RegisteredUser_RealToken()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager(User), MockHelpers.MockUserManager(User, "realToken"));
                // Act
                var result = await controller.SendCode(SendCodeViewModel) as RedirectToActionResult;

                //assert
                Assert.NotNull(result);
                Assert.Equal(nameof(AccountController.VerifyCode), result?.ActionName);
            }
        }

        public class VerifyCode
        {
            [Fact]
            public async void AnonymousUser()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(), MockHelpers.MockUserManager<ApplicationUser>());
                // Act
                var result = await controller.VerifyCode("Email", true, "returnurl") as ViewResult;
                //assert
                Assert.NotNull(result);
                Assert.Equal("Error", result?.ViewName);
            }

            [Fact]
            public async void RegisteredUser()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager(User), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.VerifyCode("Email", true, "returnurl") as ViewResult;

                //assert
                Assert.NotNull(result);
                Assert.NotNull(controller.ViewData.Model);
                Assert.Equal(nameof(AccountController.VerifyCode), result?.ViewName ?? nameof(AccountController.VerifyCode));
            }

            [Fact]
            public async void Post_User()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Failed), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.VerifyCode(VerifyCodeViewModel) as ViewResult;
                //assert
                Assert.NotNull(result);
                Assert.Equal(nameof(AccountController.VerifyCode), result?.ViewName ?? nameof(AccountController.VerifyCode));
            }

            [Fact]
            public async void Post_User_LockedOut()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.LockedOut), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.VerifyCode(VerifyCodeViewModel) as ViewResult;
                //assert
                Assert.NotNull(result);
                Assert.Equal("Lockout", result?.ViewName);
            }

            [Fact]
            public async void Post_User_Success()
            {
                // Arrange
                var controller = MockAccountController(MockHelpers.MockSignInManager<ApplicationUser>(SignInResult.Success), MockHelpers.MockUserManager(User));
                // Act
                var result = await controller.VerifyCode(VerifyCodeViewModel) as RedirectResult;
                //assert
                Assert.NotNull(result);
                Assert.Equal("localUrl", result?.Url);
            }
        }
    }
}