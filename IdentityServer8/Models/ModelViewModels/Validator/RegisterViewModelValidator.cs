using System;
using FluentValidation;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Models.ModelViewModels.Validator;

public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterViewModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.Register.Email.REQUIRED)
            .EmailAddress().WithMessage(Statuses.ViewModelValidator.Error.Register.Email.FORMAT);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.Login.Password.REQUIRED)
            .MinimumLength(6).WithMessage(Statuses.ViewModelValidator.Error.Login.Password.FORMAT);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.Register.FirstName.REQUIRED);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.Register.LastName.REQUIRED);
    }
}
