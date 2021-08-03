using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(f => f.CategoryId)
                .IsRequired();

            builder.Ignore(p => p.IsIncludedInPlan);

        }

    }
}
