using FluentValidation;
using TaskFlow.Application.Commands.Projects;

namespace TaskFlow.Application.Validators;

public sealed class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(command => command.OrganizationId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(1000);
    }
}
