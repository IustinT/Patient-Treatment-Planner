
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICU.Data.Models
{
    public class DataTransferObject
    {
        public Patient Patient { get; set; }

        public IEnumerable<Goal> Goals { get; set; }

    }

    public class Patient
    {
        public long? Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public string Ward { get; set; }
        public string Hospital { get; set; }

    }

    public class CPAX
    {
        public Guid Id { get; set; }
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
    }

    public class PatientCPAXMap
    {
        public Guid Id { get; set; }
        public Guid CpaxId { get; set; }
        public CPAX CPAX { get; set; }
        public long PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The Goal CPAX values for the <see cref="Patient"/>
        /// </summary>
        public bool IsGoal { get; set; }

        /// <summary>
        /// The latest CPAX values for the <see cref="Patient"/>
        /// </summary>
        public bool IsLatest { get; set; }
    }

    public class Goal
    {
        public Guid Id { get; set; }
        public long PatientId { get; set; }

        [JsonIgnore]
        public Patient Patient { get; set; }
        public string Value { get; set; }
        public bool? IsMainGoal { get; set; }

    }

    public class Achievemt
    {
        public Guid Id { get; set; }
        public long PatientId { get; set; }
        public Patient Patient { get; set; }
        public string Value { get; set; }
        public DateTime DateTime { get; set; }
    }

}
