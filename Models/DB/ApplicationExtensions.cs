using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;

namespace DBC.Models.DB
{
    public static class ApplicationExtensions
    {
        public static void EnsureMigrationsApplied(this IApplicationBuilder app)
        {
            ApplicationContext context = app.ApplicationServices.GetService<ApplicationContext>();
            //context.Database.AsRelational().ApplyMigrations();
        }

        public async static void EnsureSampleData(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<ApplicationContext>();
            var userManager = app.ApplicationServices.GetService<UserManager<ApplicationUser>>();
            var roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole>>();

            if (context.AllMigrationsApplied())
            {
                foreach(var role in roleManager.Roles)
                {
                    Console.WriteLine(role.Name);
                }
                foreach (string role in Enum.GetNames(typeof(Roles)))
                {
                    if(!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }

                }
                
                string password = "P@ssword!";
                await CreateUserIfNotExist(userManager, "thom@medella.nl", password, Roles.Admin.ToString(),"Google", "110018662340682049067");
                await CreateUserIfNotExist(userManager, "Bobbie@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "KapiteinArchibaldHaddock@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "TrifoniusZonnebloem@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "JansenenJanssen@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "BiancaCastofiore@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "Nestor@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "SerafijnLampion@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "Rastapopoulos@kuifje.be", password, Roles.Admin.ToString());
                await CreateUserIfNotExist(userManager, "Sponsz@kuifje.be", password, Roles.Admin.ToString());
            }
            else
                throw new System.Exception("Not all migration are applied");
        }

        private static async System.Threading.Tasks.Task CreateUserIfNotExist(UserManager<ApplicationUser> userManager, string Email, string password, string role,string loginProvider = null, string providerKey = null)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = Email, Email = Email };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
                await userManager.SetLockoutEnabledAsync(user, false);
                if (loginProvider != null && providerKey != null)
                {
                    await userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, ""));
                }
                else
                {
                    await userManager.SetLockoutEnabledAsync(user, true);
                    await userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(2016, 01, 01, 23, 0, 0, new TimeSpan(-2, 0, 0)));
                }
            }
        }
    }
}