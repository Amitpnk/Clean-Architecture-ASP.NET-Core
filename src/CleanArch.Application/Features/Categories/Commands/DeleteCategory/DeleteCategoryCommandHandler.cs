using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(IMapper mapper, IGenericRepositoryAsync<Category> categoryRepository)
    : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IMapper _mapper = mapper;

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryToDelete = await categoryRepository.GetByIdAsync(request.CategoryId);

        if (categoryToDelete == null)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        await categoryRepository.DeleteAsync(categoryToDelete);

        return Unit.Value;
    }
}