using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DBC.Models.DB
{
    public class ApplicationUser : IdentityUser
    {
        public override bool TwoFactorEnabled { get; set; } = false;
        public override bool EmailConfirmed { get; set; } = true;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public override DateTimeOffset? LockoutEnd { get { return base.LockoutEnd; } set { base.LockoutEnd = value; } }
        [Required]
        [ScaffoldColumn(false)]
        [Display(Description = "Email adress is used for login")]
        public override string Email { get { return base.Email; } set { base.Email = value; } }
    }
}