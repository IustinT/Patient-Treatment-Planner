using FluentValidation;

using ICU.Data.Models;

using System;

namespace ICU.API.Models.FluentValidation
{
    public class GoalValidator : AbstractValidator<Goal>
    {
        public GoalValidator()
        {
            RuleFor(p => p.PatientId)
                .NotEmpty();

            RuleFor(p => p.Value)
                .NotEmpty()
                .Length(2, 450);

            RuleFor(p => p.IsMainGoal)
                .NotNull();

        }
    }
}
