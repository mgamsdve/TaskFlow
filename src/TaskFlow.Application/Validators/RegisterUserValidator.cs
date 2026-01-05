using FluentValidation;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Validators;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(request => request.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(request => request.Role)
            .IsInEnum();
    }
}
