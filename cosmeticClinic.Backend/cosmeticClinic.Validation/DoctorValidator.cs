using cosmeticClinic.Entities;
using FluentValidation;

namespace cosmeticClinic.Validation;

public class DoctorValidator : AbstractValidator<Doctor>
{
    public DoctorValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("First name is required and must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Last name is required and must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email address is required");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in a valid international format");

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Specialization is required and must not exceed 100 characters");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("License number is required and must not exceed 50 characters");

        RuleFor(x => x.WorkingHours)
            .NotEmpty()
            .WithMessage("Working hours must be specified");

        RuleForEach(x => x.WorkingHours).ChildRules(workingHours =>
        {
            workingHours.RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("Start time must be before end time");
        });
    }
}