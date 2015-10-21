using System;
using System.Linq;
using GGZDBC.Models.DBCModel.Afleiding;
using GGZDBC.Models.DBCModel.Registraties;
using GGZDBC.Models.DBCModel.Testset;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Migrations;
using DBC.Models;

namespace DBC.Models.DB
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Beroep> Beroep { get; set; }
        public DbSet<Activiteit> Activiteit { get; set; }
        public DbSet<ActiviteitAndTarief> ActiviteitAndTarief { get; set; }
        public DbSet<Hoofdberoep> Hoofdberoep { get; set; }
        public DbSet<Cirquit> Cirquit { get; set; }
        public DbSet<Diagnose> Diagnose { get; set; }
        public DbSet<Prestatiecode> Prestatiecode { get; set; }
        public DbSet<Productgroep> Productgroep { get; set; }
        public DbSet<Redensluiten> Redensluiten { get; set; }
        public DbSet<Zorgtype> Zorgtype { get; set; }
        public DbSet<Aanspraakbeperking> Aanspraakbeperking { get; set; }
        public DbSet<Beslisboom> Beslisboom { get; set; }

        public DbSet<Patient> Patient { get; set; }
        public DbSet<Zorgtraject> Zorgtraject { get; set; }
        public DbSet<DBCs> DBC { get; set; }
        public DbSet<Tijdschrijven> Tijdschrijven { get; set; }
        public DbSet<OverigeDiagnose> OverigeDiagnose { get; set; }
        public DbSet<DBCTestset> DBCTestset { get; set; }
        public DbSet<Oganization> Zorginstelling { get; set; }
        public DbSet<Department> Zorgafdeling { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            GGZDBC.Models.DBCModel.Registraties.Aanspraakbeperking.OnModelCreating(builder);
            builder.Entity<Activiteit>().Index(p => new { p.Code, p.branche_indicatie });
            builder.Entity<ActiviteitAndTarief>().Index(p => p.declaratiecode);
            builder.Entity<Cirquit>().Index(p => new { p.Code, p.branche_indicatie });
            builder.Entity<Beroep>().Index(p => new { p.Code, p.branche_indicatie });
            builder.Entity<Beslisboom>().HasKey(p => new { p.knoopNummer, p.Begindate });
            builder.Entity<DBCs>().HasKey(p => p.DBCID);
            builder.Entity<DBCs>().Index(p => p.ZorgtrajectIDExtern);
            builder.Entity<DBCs>().Index(p => p.DBCIDExtern);
            builder.Entity<DBCTestset>().HasKey(p => p.DBCIDExtern3);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public bool AllMigrationsApplied()
            => !((IAccessor<IServiceProvider>)this).GetService<IHistoryRepository>()
               .GetAppliedMigrations().Any();

    }
}