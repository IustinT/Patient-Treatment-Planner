using ICU.Data.Models;

using System;
using System.Collections.Generic;

namespace ICU.Planner.Models
{
    public class ImagesByCategoryModel : PatientImagesByCategoryId
    {
        public ImageCategory Category { get; set; }
        public List<Uri> Uris { get; set; }
    }
}
