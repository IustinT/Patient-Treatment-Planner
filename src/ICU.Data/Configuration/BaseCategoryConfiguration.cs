
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class BaseCategoryConfiguration : IEntityTypeConfiguration<BaseCategory>
    {
        public void Configure(EntityTypeBuilder<BaseCategory> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(450);

            builder
                .HasDiscriminator<int>("CategoryType")
                .HasValue<ImageCategory>(0)
                .HasValue<ExerciseCategory>(1);
        }

    }
}
