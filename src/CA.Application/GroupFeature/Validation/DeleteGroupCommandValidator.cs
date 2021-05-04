using CA.Application.GroupFeature.Commands;
using FluentValidation;

namespace CA.Application.GroupFeature.Validation
{
    public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
    {
        public DeleteGroupCommandValidator()
        {
            RuleFor(v => v.GroupId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

        }
    }
}
