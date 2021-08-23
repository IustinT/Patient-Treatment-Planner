using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class PatientExerciseConfiguration : IEntityTypeConfiguration<PatientExercise>
    {
        public void Configure(EntityTypeBuilder<PatientExercise> builder)
        {
            builder.HasKey(p => new { p.ExerciseId, p.PatientId });

            builder.HasOne(p => p.Patient);
            builder.HasOne(p => p.Exercise);

            builder.Property(p => p.Repetitions)
                .IsRequired();

            builder.HasIndex(p => new { p.PatientId, p.ExerciseId })
                .IsUnique(true);
        }

    }
}
