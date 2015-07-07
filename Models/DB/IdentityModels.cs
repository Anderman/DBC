using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace DBC.Models.DB
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public override bool TwoFactorEnabled { get; set; } = true;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public override DateTimeOffset? LockoutEnd { get; set; }

    }
    public enum Roles
    {
        Admin,
        Patient,
        Practitioner,
        Secretary,
    }
}