using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Mvc.Rendering;

namespace DBC.Models.View
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(ResourceType = typeof(loc), Name = "Email")]
        public string Email { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(ResourceType = typeof(loc), Name = "Code")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(ResourceType = typeof(loc), Name = "RememberThisBrowser")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(loc), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(loc), ErrorMessageResourceName = "TheMustBeAtLeastCharactersLong", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(loc), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(loc), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(loc), ErrorMessageResourceName = "ThePasswordAndConfirmationPasswordDoNotMatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(ResourceType = typeof(loc), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(ResourceType = typeof(loc), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(loc), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(loc), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(ResourceType = typeof(loc), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(loc), ErrorMessageResourceName = "TheMustBeAtLeastCharactersLong", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(loc), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(loc), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(loc), ErrorMessageResourceName = "ThePasswordAndConfirmationPasswordDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}