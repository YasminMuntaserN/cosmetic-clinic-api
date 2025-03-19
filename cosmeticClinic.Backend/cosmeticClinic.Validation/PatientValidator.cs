using cosmeticClinic.Entities;
using FluentValidation;

namespace cosmeticClinic.Validation;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
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

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Gender is required and must not exceed 20 characters");

        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Address is required");

        RuleFor(x => x.Address.Street)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Street address is required and must not exceed 100 characters");

        RuleFor(x => x.Address.City)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City is required and must not exceed 50 characters");

        RuleFor(x => x.Address.State)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("State is required and must not exceed 50 characters");

        RuleFor(x => x.Address.PostalCode)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Postal code is required and must not exceed 20 characters");

        RuleFor(x => x.Address.Country)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Country is required and must not exceed 50 characters");

        RuleFor(x => x.EmergencyContact)
            .NotNull()
            .WithMessage("Emergency contact is required");
        
    }
}