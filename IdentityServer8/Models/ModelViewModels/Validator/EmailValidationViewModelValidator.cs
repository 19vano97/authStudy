using System;
using FluentValidation;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Models.ModelViewModels.Validator;

public class EmailValidationViewModelValidator : AbstractValidator<EmailValidationViewModel>
{
    public EmailValidationViewModelValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(Statuses.ViewModelValidator.Error.EmailValidation.Email.REQUIRED)
            .EmailAddress().WithMessage(Statuses.ViewModelValidator.Error.EmailValidation.Email.FORMAT);
    }
}
