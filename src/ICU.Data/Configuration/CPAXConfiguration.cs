

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class CPAXConfiguration : IEntityTypeConfiguration<CPAX>
    {
        public void Configure(EntityTypeBuilder<CPAX> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(p => p.Patient)
                .WithMany(m => m.CPAXes)
                .HasForeignKey(p => p.PatientId);

            builder.Property(p => p.IsGoal)
                .IsRequired();

            builder.HasIndex(p => new { p.PatientId, p.IsGoal })
                .IsUnique(false);
        }

    }
}
