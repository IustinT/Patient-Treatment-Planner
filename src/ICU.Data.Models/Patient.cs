using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        [JsonIgnore] //ignore this as the relevant data will be in Current and Goal cpax
        public List<CPAX> CPAXes { get; set; }

        public CPAX CurrentCPAX { get; set; }

        public CPAX GoalCPAX { get; set; }

        public List<ImageCategoryWithFiles> Images { get; set; }

        public virtual List<PatientExercise> ExercisesAssignment { get; set; }

        public int? MondayExerciseTime { get; set; }
        public int? TuesdayExerciseTime { get; set; }
        public int? WednesdayExerciseTime { get; set; }
        public int? ThursdayExerciseTime { get; set; }
        public int? FridayExerciseTime { get; set; }
        public int? SaturdayExerciseTime { get; set; }
        public int? SunExerciseTime { get; set; }
    }

    public class CPAX
    {
        public Guid? Id { get; set; }

        public long? PatientId { get; set; }

        [JsonIgnore]
        public virtual Patient Patient { get; set; }

        public DateTime? DateTime { get; set; }

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

        /// <summary>
        /// Compares all values of both objects' properties.
        /// </summary>
        /// <param name="obj">The other <see cref="CPAX"/> object.</param>
        /// <returns><see langword="True"/> if all properties have same values in both objects. <see langword="False"/> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is CPAX other
                   && Grip == other.Grip
                   && Respiratory == other.Respiratory
                   && Cough == other.Cough
                   && BedMovement == other.BedMovement
                   && DynamicSitting == other.DynamicSitting
                   && StandingBalance == other.StandingBalance
                   && SitToStand == other.SitToStand
                   && BedToChair == other.BedToChair
                   && Stepping == other.Stepping
                   && Transfer == other.Transfer;
        }

        /// <summary>
        /// Indicates if all properties have default value of zero.
        /// </summary>
        /// <returns><see langword="True"/> if all properties have value of zero. <see langword="False"/> otherwise.</returns>
        public bool IsEmpty() =>
            (Grip + Respiratory + Cough + BedMovement
             + DynamicSitting + StandingBalance + SitToStand + BedToChair
             + Stepping + Transfer) is 0;
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

    public class ImageCategory : BaseCategory
    { }

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

    public class Exercise
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public ExerciseCategory Category { get; set; }

        public string Aim { get; set; }
        public string Instructions { get; set; }
        public string Variations { get; set; }
        public string Precautions { get; set; }

        /// <summary>
        /// Not Mapped to database.
        /// A flag indicating if this <see cref="Exercise"/> is included in the <see cref="Patient"/>'s plan.
        /// </summary>
        public bool IsIncludedInPlan { get; set; }
    }

    public class ExerciseCategory : BaseCategory
    { }

    public abstract class BaseCategory
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool Deleted { get; set; }
    }

    /// <summary>
    /// A map table between <see cref="Patient"/> and <see cref="Exercise"/>,
    /// indicating the number of <see cref="PatientExercise.Repetitions"/>.
    /// </summary>
    public class PatientExercise
    {
        public long PatientId { get; set; }

        [JsonIgnore]
        public virtual Patient Patient { get; set; }

        public long ExerciseId { get; set; }

        [JsonIgnore]
        public virtual Exercise Exercise { get; set; }

        public int Repetitions { get; set; }
    }

    public class CpaxDTO
    {
        public CPAX CurrentCpax { get; set; }
        public CPAX GoalCpax { get; set; }
    }
}
