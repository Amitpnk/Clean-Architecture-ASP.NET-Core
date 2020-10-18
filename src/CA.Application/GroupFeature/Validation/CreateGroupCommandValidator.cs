using CA.Application.GroupFeature.Commands;
using FluentValidation;

namespace CA.Application.GroupFeature.Validation
{
    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupCommandValidator()
        {
            RuleFor(x => x.Description).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Name).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.");

        }
    }
}
