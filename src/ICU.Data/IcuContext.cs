using ICU.Data.Configuration;
using ICU.Data.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Reflection;

/*
    EntityFrameworkCore\Add-Migration -Name Initial -Context ICU.Data.IcuContext -StartupProject ICU.API -Project ICU.Data

    EntityFrameworkCore\Remove-Migration -Context ICU.Data.IcuContext -StartupProject ICU.API -Project ICU.Data

    EntityFrameworkCore\Update-Database -Context ICU.Data.IcuContext -StartupProject ICU.API -Project ICU.Data

 */

namespace ICU.Data
{
    public class IcuContext : DbContext
    {
        static readonly Assembly assembly;

        static IcuContext()
        {
            assembly = Assembly.GetAssembly(typeof(IcuContext));
        }

        public IcuContext(DbContextOptions<IcuContext> options)
            : base(options)
        { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<ImageCategory> ImageCategories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
