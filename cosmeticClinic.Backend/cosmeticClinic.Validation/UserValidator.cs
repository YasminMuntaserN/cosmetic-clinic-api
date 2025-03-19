using cosmeticClinic.Entities;

namespace cosmeticClinic.Validation;

using FluentValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email is required");

        RuleFor(x => x.PasswordHash)
            .NotEmpty()
            .WithMessage("Password hash is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("First name is required and must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Last name is required and must not exceed 50 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(30)
            .WithMessage("Role is required and must not exceed 30 characters");

        RuleFor(x => x.CreatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("CreatedAt must not be in the future");

        RuleFor(x => x.LastLogin)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.LastLogin.HasValue)
            .WithMessage("LastLogin cannot be in the future");

        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage("IsActive status must be specified");
    }
}
