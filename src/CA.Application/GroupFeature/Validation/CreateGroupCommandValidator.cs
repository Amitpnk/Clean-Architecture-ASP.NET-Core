using CA.Application.GroupFeature.Commands;
using CA.Application.GroupFeature.Service;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Application.GroupFeature.Validation
{
    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        private readonly IGroupRepositoryAsync _groupRepositoryAsync;
        public CreateGroupCommandValidator(IGroupRepositoryAsync groupRepositoryAsync)
        {
            _groupRepositoryAsync = groupRepositoryAsync;

            RuleFor(x => x.Description).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Name).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
            .MustAsync(IsUniqueName).WithMessage("{PropertyName} already exists.");
        }

        private async Task<bool> IsUniqueName(string name, CancellationToken cancellationToken)
        {
            return await _groupRepositoryAsync.IsUniqueGroupNameAsync(name);
        }
    }
}
