using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.CQRS;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(IGenericRepositoryAsync<Category> categoryRepository)
    : CommandHandler<DeleteCategoryCommand>
{
    protected override async Task HandleAsync(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryToDelete = await categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException(nameof(Category), request.CategoryId);

        await categoryRepository.DeleteAsync(categoryToDelete);
    }
}
