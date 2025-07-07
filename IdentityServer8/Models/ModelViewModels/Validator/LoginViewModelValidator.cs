using System;
using FluentValidation;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Models.ModelViewModels.Validator;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.Login.Email.REQUIRED)
            .EmailAddress().WithMessage(Statuses.ViewModelValidator.Error.Login.Email.FORMAT);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.Login.Password.REQUIRED)
            .MinimumLength(6).WithMessage(Statuses.ViewModelValidator.Error.Login.Password.FORMAT);
    }
}
