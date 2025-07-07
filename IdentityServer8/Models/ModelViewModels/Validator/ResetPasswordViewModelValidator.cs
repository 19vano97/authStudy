using System;
using FluentValidation;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Models.ModelViewModels.Validator;

public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
{
    public ResetPasswordViewModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.Email.REQUIRED)
            .EmailAddress().WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.Email.FORMAT);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.NewPassword.REQUIRED)
            .MinimumLength(6).WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.NewPassword.FORMAT);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.ConfirmPassword.REQUIRED)
            .MinimumLength(6).WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.ConfirmPassword.FORMAT)
            .Equal(x => x.NewPassword).WithMessage(Statuses.ViewModelValidator.Error.ResetPassword.PASSWORDS_ARE_NOT_EQUAL);
    }
}
