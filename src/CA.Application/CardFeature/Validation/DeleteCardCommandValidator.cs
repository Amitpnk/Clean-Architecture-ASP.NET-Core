using CA.Application.CardFeature.Commands;
using FluentValidation;

namespace CA.Application.CardFeature.Validation
{
    public class DeleteCardCommandValidator : AbstractValidator<DeleteCardCommand>
    {
        public DeleteCardCommandValidator()
        {
            RuleFor(v => v.CardId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

        }
    }
}
