
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(w => w.Id)
                .IsRequired()
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.Ward)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.Hospital)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.AdmissionDate)
                .IsRequired();

            builder.Ignore(i => i.Images);

            builder.Ignore(i => i.CurrentCPAX);
            builder.Ignore(i => i.GoalCPAX);

            builder.Ignore(i => i.MiniGoals);
            builder.Ignore(i => i.MainGoal);
        }

    }
}
