using CA.Application.CardFeature.Commands;
using FluentValidation;

namespace CA.Application.CardFeature.Validation
{
    public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
    {
        public CreateCardCommandValidator()
        {
            RuleFor(x => x.Synonmys)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Description).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Meaning).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Chapter)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0.");

            RuleFor(x => x.Verse)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0.");

        }
    }
}
