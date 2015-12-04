using System.Linq.Dynamic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Mvc.JQuery.Datatables;
using DBC.Models;
using System.Linq.Expressions;
using System;
using System.Collections;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using DBC.Models.DB;
using Microsoft.AspNet.Authorization;
using System.Diagnostics;
using DBC.Services;
using DBC.ViewModels.Account;
using Microsoft.Extensions.Localization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DBC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;
        private IStringLocalizer<AccountController> T;
        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IEmailTemplate emailTemplate,
            IStringLocalizer<AccountController> localizer)
        {
            DbContext = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
            T = localizer;
        }

        public ApplicationDbContext DbContext { get; }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            //var x = DbContext.Users.Include(u => u.Logins).Select(l => l.Logins).ToArray();
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Index([FromBody]DataTablesRequest dTRequest)
        {
            var userRoleNames = DbContext.UserRoles.Join(DbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { r.Name, ur.UserId });
            var z = (from u in DbContext.Users
                     select new
                     {
                         u.Id,
                         u.Email,
                         u.EmailConfirmed,
                         u.UserName,
                         u.TwoFactorEnabled,
                         u.LockoutEnd,
                         Logins = DbContext.UserLogins.Where(ul => ul.UserId == u.Id).Select(ul => new { LoginProvider = ul.LoginProvider }),//u.Logins.Select(l => new { LoginProvider = l.LoginProvider }).ToArray(), // rewrite to workarounf bug
                         Roles = userRoleNames.Where(ur => ur.UserId == u.Id).Select(ur => new { Name = ur.Name }),
                     });
            var zz = z.ToArray();
            return new Mvc.JQuery.Datatables.DataTables().GetJSonResult(
                z
                , dTRequest);
        }
        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, confirmCode = code }, protocol: HttpContext.Request.Scheme);
                        var body = await _emailTemplate.RenderViewToString(@"/Views/Email/ActivateEmail", new ActivateEmail() { Emailaddress = user.Email, Callback = callbackUrl });
                        await _emailSender.SendEmailAsync(user.Email, T["Confirm your account"], body);
                        return new JsonResult(new {success= "dbc.snackbar", data="An Email is send to user" });
                    }
                    return new EmptyResult();
                }
                else
                {
                    AddErrors(result);
                }
            }
            return PartialView(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult Edit(string Id)
        {
            //var dummy = DbContext.Users.Include(u => u.Logins).ToArray();
            var user = DbContext.Users.Include(u => u.Logins).Include(u => u.Claims).Include(u => u.Roles).FirstOrDefault(u => u.Id == Id);
            return PartialView(user);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                user.AccessFailedCount = model.AccessFailedCount;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;
                user.LockoutEnabled = model.LockoutEnabled;
                user.LockoutEnd = model.LockoutEnd;
                user.PhoneNumber = model.PhoneNumber;
                user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                user.TwoFactorEnabled = model.TwoFactorEnabled;
                user.UserName = model.UserName;
                await _userManager.UpdateAsync(user);

                DbContext.SaveChanges();
                return new EmptyResult();
            }
            return PartialView(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmail(string Id)
        {

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return new JsonResult($"Error: The user does not exist");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, confirmCode = code }, protocol: HttpContext.Request.Scheme);
            var body = await _emailTemplate.RenderViewToString(@"/Views/Email/ActivateEmail", new ActivateEmail() { Emailaddress = user.Email, Callback = callbackUrl });
            await _emailSender.SendEmailAsync(user.Email, T["Confirm your account"], body);
            return new JsonResult($"Email is send to {user.Email}");
        }

        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult Delete(string Id)
        {
            var user = DbContext.Users.Single(u => u.Id == Id);
            return PartialView(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            await _userManager.DeleteAsync(user);
            return new EmptyResult();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
