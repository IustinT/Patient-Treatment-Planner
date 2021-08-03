
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ICU.Data.Models;

namespace ICU.Data.Configuration
{
    public class ImageCategoryConfiguration : IEntityTypeConfiguration<ImageCategory>
    {
        public void Configure(EntityTypeBuilder<ImageCategory> builder)
        {
        }

    }
}
