using CA.Application.GroupFeature.Commands;
using FluentValidation;

namespace CA.Application.GroupFeature.Validation
{
    public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
    {
        public UpdateGroupCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Description).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.");

        }
    }
}
