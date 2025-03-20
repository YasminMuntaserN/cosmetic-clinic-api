using cosmeticClinic.Entities;
using FluentValidation;

namespace cosmeticClinic.Validation;

public class AppointmentValidator : AbstractValidator<Appointment>
{
    public AppointmentValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty()
            .WithMessage("Doctor ID is required");

        RuleFor(x => x.TreatmentId)
            .NotEmpty()
            .WithMessage("Treatment ID is required");

        RuleFor(x => x.ScheduledDateTime)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Scheduled date and time must be in the future");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(480) 
            .WithMessage("Duration must be between 1 and 480 minutes");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid appointment status");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => x.Notes != null)
            .WithMessage("Notes must not exceed 1000 characters");

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .MaximumLength(500)
            .When(x => x.Status == AppointmentStatus.Cancelled)
            .WithMessage("Cancellation reason is required when status is Cancelled and must not exceed 500 characters");
    }
}