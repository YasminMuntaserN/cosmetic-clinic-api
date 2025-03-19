using cosmeticClinic.Entities;
using FluentValidation;

namespace cosmeticClinic.Validation;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
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

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.Manufacturer)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Manufacturer is required and must not exceed 100 characters");

        RuleFor(x => x.Ingredients)
            .NotEmpty()
            .WithMessage("At least one ingredient must be specified");

        RuleForEach(x => x.Ingredients)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Each ingredient must not be empty and must not exceed 100 characters");

        RuleFor(x => x.Usage)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Usage instructions are required and must not exceed 1000 characters");

        RuleFor(x => x.SideEffects)
            .NotEmpty()
            .WithMessage("At least one side effect must be specified");

        RuleForEach(x => x.SideEffects)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Each side effect must not be empty and must not exceed 200 characters");
        
    }
}