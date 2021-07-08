

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(p => p.Patient)
                .WithMany(m => m.Goals)
                .HasForeignKey(p => p.PatientId);

            builder.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.IsMainGoal)
                .IsRequired();

            builder.HasIndex(p => new { p.PatientId, p.IsMainGoal })
                .IsUnique(false);
        }

    }
}
