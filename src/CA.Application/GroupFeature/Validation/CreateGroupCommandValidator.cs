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
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            //.MustAsync(IsUniqueBarcode).WithMessage("{PropertyName} already exists.");

        }

        //private async Task<bool> IsUniqueBarcode(string barcode, CancellationToken cancellationToken)
        //{
        //    return await productRepository.IsUniqueBarcodeAsync(barcode);
        //}

    }
}
