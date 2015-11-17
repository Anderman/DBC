using System.Linq;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Internal;

namespace DBC.Models.DB
{
    // Add profile data for application users by adding properties to the ApplicationUser class

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<Localizations> Localizations { get; set; }
        private string _connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _connectionString = ((SqlServerOptionsExtension)optionsBuilder.Options.Extensions.First()).ConnectionString;
            //Console.WriteLine($"ApplicationDbContext{_connectionString}");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            
            base.OnModelCreating(builder);
            //builder.Entity<Localizations>().HasKey(p => new { p.Key, p.Culture });

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public bool AllMigrationsApplied()
        {
            return true; //!this.GetService<IHistoryRepository>().GetAppliedMigrations().Any();
        }
    }
}