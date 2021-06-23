
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICU.Data.Models
{
    public class Patient
    {
        public long? Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public string Ward { get; set; }
        public string Hospital { get; set; }

        [JsonIgnore] //ignore as relevant data will be in MiniGoals and MainGoal
        public List<Goal> Goals { get; set; }
        public List<Goal> MiniGoals { get; set; }
        public Goal MainGoal { get; set; }

        public List<Achievement> Achievemts { get; set; }

        [JsonIgnore]//ignore this as the relevant data will be in Current and Goal cpax
        public List<CPAX> CPAXes { get; set; }

        public CPAX CurrentCPAX { get; set; }

        public CPAX GoalCPAX { get; set; }

        public List<ImageCategoryWithFiles> Images { get; set; }
    }

    public class CPAX
    {
        public Guid Id { get; set; }

        public long PatientId { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }
        public DateTime DateTime { get; set; }

        public int Grip { get; set; }
        public int Respiratory { get; set; }
        public int Cough { get; set; }
        public int BedMovement { get; set; }
        public int DynamicSitting { get; set; }
        public int StandingBalance { get; set; }
        public int SitToStand { get; set; }
        public int BedToChair { get; set; }
        public int Stepping { get; set; }
        public int Transfer { get; set; }

        public bool IsGoal { get; set; }
    }

    public class Goal
    {
        public Guid Id { get; set; }
        public long PatientId { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }

        public string Value { get; set; }
        public bool? IsMainGoal { get; set; }

    }

    public class Achievement
    {
        public Guid Id { get; set; }
        public long PatientId { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }

        public string Value { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class ImageCategory
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool Deleted { get; set; }
    }

    public class ImageCategoryWithFiles : ImageCategory
    {
        public List<ImageFile> ImageFiles { get; set; }
    }

    public class ImageFile
    {
        public long PatientId { get; set; }
        public int CategoryId { get; set; }
        public string FileName { get; set; }
        public string Uri { get; set; }
    }
}
