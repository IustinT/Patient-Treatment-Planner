using ICU.Data.Models;

using System;
using System.Collections.Generic;

namespace ICU.Planner.Models
{
    public class ImagesByCategoryModel : ImageCategoryWithFiles
    {
        public ImageCategory Category { get; set; }
        public List<Uri> Uris { get; set; }
    }
}
