using FluentValidation;
using TaskFlow.Application.Commands.Tasks;

namespace TaskFlow.Application.Validators;

public sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(command => command.ListId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);

        RuleFor(command => command.Priority)
            .IsInEnum();

        RuleFor(command => command.Position)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.DueDateUtc)
            .Must(HaveTimeZoneInfo)
            .WithMessage("DueDateUtc must include timezone information when provided.");
    }

    private static bool HaveTimeZoneInfo(DateTime? dueDateUtc)
    {
        return !dueDateUtc.HasValue || dueDateUtc.Value.Kind != DateTimeKind.Unspecified;
    }
}
