using FluentValidation;

using ICU.Data.Models;

using System;

namespace ICU.API.Models.FluentValidation
{
    public class CpaxValidator : AbstractValidator<CPAX>
    {
        public CpaxValidator()
        {
            RuleFor(p => p.PatientId)
                .NotEmpty();

            RuleFor(p => p.Respiratory)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.BedMovement)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.BedToChair)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.Cough)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.DynamicSitting)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.Grip)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.SitToStand)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.StandingBalance)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.Stepping)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.Transfer)
                .GreaterThanOrEqualTo(0);

        }
    }
}
