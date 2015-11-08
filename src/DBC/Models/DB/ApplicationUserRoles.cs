using Microsoft.AspNet.Identity.EntityFramework;

namespace DBC.Models
{
    public class ApplicationUserRoles : IdentityUserRole<string>
    {
        public virtual ApplicationUser Role { get; set; }
    }
    public enum Roles
    {
        Admin,
        Patient,
        Practitioner,
        Secretary,
    }
}
