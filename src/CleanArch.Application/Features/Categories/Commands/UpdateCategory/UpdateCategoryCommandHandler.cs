using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.CQRS;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MapsterMapper;

namespace CleanArch.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(IMapper mapper, IGenericRepositoryAsync<Category> categoryRepository)
    : CommandHandler<UpdateCategoryCommand>
{
    protected override async Task HandleAsync(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryToUpdate = await categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException(nameof(Category), request.CategoryId);

        var validator = new UpdateCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count > 0)
            throw new ValidationException(validationResult);

        mapper.Map(request, categoryToUpdate);

        await categoryRepository.UpdateAsync(categoryToUpdate);
    }
}
