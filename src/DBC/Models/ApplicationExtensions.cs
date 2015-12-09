using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace DBC.Models.DB
{
    public static class ApplicationExtensions
    {
        public static void EnsureMigrationsApplied(this IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.GetService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        public static async Task EnsureSampleData(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<ApplicationDbContext>();
            var userManager = app.ApplicationServices.GetService<UserManager<ApplicationUser>>();
            var roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole>>();


            if (context.AllMigrationsApplied())
            {
                foreach (string role in Enum.GetNames(typeof(Roles)))
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }

                }

                const string password = "P@ssw0rd!";
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "thom@medella.nl" }, password, Roles.Admin.ToString(), "Google", "110018662340682049067");
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "Bobbie@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "KapiteinArchibaldHaddock@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "TrifoniusZonnebloem@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "JansenenJanssen@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "BiancaCastofiore@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "Nestor@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "SerafijnLampion@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "Rastapopoulos@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser { UserName = "Sponsz@kuifje.be" }, password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, new ApplicationUser
                {
                    UserName = "lockout@test.nl",
                    LockoutEnabled = true,
                    LockoutEnd = DateTime.Now.AddDays(1)

                }, password, Roles.Admin.ToString());
                var result = await userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "confirm@test.nl",
                    EmailConfirmed = false,
                    TwoFactorEnabled = true
                }, password);
            }
            else
                throw new System.Exception("Not all migration are applied");
        }

        private static async Task<ApplicationUser> CreateUserIfNotExist(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role, string loginProvider = null, string providerKey = null)
        {
            //Debugger.Launch();
            user.EmailConfirmed = true;
            user.Email = user.Email ?? user.UserName;
            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new ApplicationException(string.Join("\n", result.Errors.Select(a => a.Description).ToArray()));
                }
                await userManager.AddToRoleAsync(user, role);
                if (loginProvider != null && providerKey != null)
                {
                    await userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, ""));
                }
            }
            return user;
        }
    }
}