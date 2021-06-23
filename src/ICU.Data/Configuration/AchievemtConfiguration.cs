

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class AchievemtConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(p => p.Patient)
                .WithMany(m => m.Achievemts)
                .HasForeignKey(p => p.PatientId);

            builder.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.DateTime)
                .IsRequired();

        }

    }
}
