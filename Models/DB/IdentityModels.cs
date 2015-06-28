using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace DBC.Models.DB
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public override bool TwoFactorEnabled { get; set; } = true;
    }
}