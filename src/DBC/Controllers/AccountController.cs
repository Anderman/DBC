using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using DBC.Models;
using DBC.Services;
using DBC.ViewModels.Account;
using Microsoft.Extensions.Localization;
using DBC.Models.DB;

namespace DBC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IStringLocalizer<AccountController> T;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            ApplicationDbContext applicationDbContext,
            IEmailTemplate emailTemplate,
            IStringLocalizer<AccountController> localizer)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _applicationDbContext = applicationDbContext;
            _emailTemplate = emailTemplate;
            T = localizer;

        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var u = _applicationDbContext.Users.ToList();

                var user = await _userManager.FindByEmailAsync(model.Email);
                var result = user==null?
                    SignInResult.Failed:
                    await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, T["Invalid login attempt."]);
                    if (result.IsNotAllowed)
                        ModelState.AddModelError(string.Empty, T["Email or phonenumber not confirmed"]);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //// UserManager create accounts 
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await _userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //            // Send an email with this link
        //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
        //            var body = await _emailTemplate.RenderViewToString(@"/Views/Email/ActivateEmail", new ActivateEmail() { Emailaddress = user.Email, Callback = callbackUrl });
        //            await _emailSender.SendEmailAsync(model.Email, T["Confirm your account"], body);
        //            //await _signInManager.SignInAsync(user, isPersistent: false); //comment out do not log on a
        //            return RedirectToAction(nameof(HomeController.Index), "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string error = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                if (error != null)
                {
                    ModelState.AddModelError("", T["The external login service gives an error: {0}.", error]);
                    return View("Login");
                }
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (!result.Succeeded && !result.IsLockedOut && !result.RequiresTwoFactor)
            {
                //Local only Email exist and it not yet confirm or registed as external login. 
                //if a user has proven he own this email address then confirm the registration and register the external login to the user
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.UserName))
                    {
                        user.UserName = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Name);
                    }
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    if ((await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)) == null)
                    {
                        await _userManager.AddLoginAsync(user, info);
                    }
                    result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                }
            }

            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {0} provider.", info.ProviderDisplayName);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", T["Confirmation of email or phonenumber is required before you can login"]);
                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }
            else
            {
                // If the user does not have an account, then ask the user to login with a different account.
                ModelState.AddModelError("", T["Your {0} account is unknown to the application. You can add your {0} account in your profile.", info.LoginProvider]);
                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }
        }


        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string confirmCode)
        {
            if (userId == null || confirmCode == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, confirmCode);
            if (!result.Succeeded)
            {
                AddErrors(result);

                return View("Error");
            }
            var model = new ResetPasswordViewModel()
            {
                Code = await _userManager.GeneratePasswordResetTokenAsync(user),
                Email = user.Email
            };
            return View(nameof(ConfirmEmail), model);
        }
        // GET: /Account/ConfirmEmail
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                if (signInResult.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode));
                }
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, T["Invalid login attempt."]);
                    if (signInResult.IsNotAllowed)
                        ModelState.AddModelError(string.Empty, T["Email or phonenumber not confirmed"]);
                    return View(model);
                }
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View(nameof(ForgotPasswordConfirmation), model);
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext?.Request?.Scheme);
                var body = await _emailTemplate.RenderViewToString(@"/Views/Email/ResetPasswordEmail", new ActivateEmail() { Emailaddress = user.Email, Callback = callbackUrl });
                await _emailSender.SendEmailAsync(model.Email, T["Reset password"], body);
                //show a screen that the use should check hit email
                return View(nameof(ForgotPasswordConfirmation), model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email) ?? await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                _logger.LogInformation("Could not find user with email {email}", model.Email);
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            // Require that the user has already logged in via username/password or external login
            //TwoFactorUserId cookie is set after login. So get that user from cookie	
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            //factorOptions.Add(new SelectListItem() { Text = "SMS", Value = "SMS" });
            return View(new SendCodeViewModel { SelectedProvider = factorOptions.First().Value, Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = T["Your security code is: {0}", code];
            if (model.SelectedProvider == "Email")
            {
                var e = await _userManager.GetEmailAsync(user);
                await _emailSender.SendEmailAsync(e, T["Security Code"], message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", T["Invalid code."]);
                return View(model);
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
