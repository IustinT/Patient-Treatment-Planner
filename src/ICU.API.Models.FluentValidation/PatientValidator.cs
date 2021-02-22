using FluentValidation;

using ICU.Data.Models;

using System;

namespace ICU.API.Models.FluentValidation
{
    public class PatientValidator : AbstractValidator<Patient>
    {
        public PatientValidator()
        {
            RuleFor(patient => patient.PhoneNumber)
              .NotEmpty()
              .Length(11);

            RuleFor(patient => patient.Name)
                .NotEmpty()
                .Length(2, 450);

            RuleFor(patient => patient.AdmissionDate)
                .NotNull()
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(patient => patient.Hospital)
                .NotEmpty()
                .Length(2, 450);

            RuleFor(patient => patient.Ward)
                .NotEmpty()
                .Length(2, 450);

        }


    }
}
