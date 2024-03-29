﻿using ICU.Data.Models;

using Microsoft.EntityFrameworkCore;

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
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ImageCategory> ImageCategories { get; set; }
        public DbSet<CPAX> CPAXes { get; set; }
        public DbSet<ExerciseCategory> ExerciseCategories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<PatientExercise> PatientExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
