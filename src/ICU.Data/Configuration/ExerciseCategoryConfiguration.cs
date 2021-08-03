
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class ExerciseCategoryConfiguration : IEntityTypeConfiguration<ExerciseCategory>
    {
        public void Configure(EntityTypeBuilder<ExerciseCategory> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(450);

        }

    }
}
