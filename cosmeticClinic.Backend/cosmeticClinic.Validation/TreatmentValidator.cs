using cosmeticClinic.Entities;
using FluentValidation;

namespace cosmeticClinic.Validation;

public class TreatmentValidator : AbstractValidator<Treatment>
{
    public TreatmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Description is required and must not exceed 1000 characters");
        

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration Minutes cannot be negative");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price cannot be negative");

        RuleFor(x => x.RequiredEquipment)
            .NotEmpty()
            .WithMessage("At least one Required Equipment must be specified");

        RuleForEach(x => x.RequiredEquipment)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Each Required Equipment must not be empty and must not exceed 100 characters");
        
        RuleFor(x => x.AfterCare)
            .NotEmpty()
            .WithMessage("At least one After Care must be specified");

        RuleForEach(x => x.AfterCare)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Each Required After Care must not be empty and must not exceed 100 characters");
        
    }
}