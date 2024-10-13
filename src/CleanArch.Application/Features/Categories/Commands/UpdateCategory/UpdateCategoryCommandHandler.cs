using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(IMapper mapper, IGenericRepositoryAsync<Category> categoryRepository)
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {

        var categoryToUpdate = await categoryRepository.GetByIdAsync(request.CategoryId);

        if (categoryToUpdate == null)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var validator = new UpdateCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Count > 0)
            throw new ValidationException(validationResult);

        mapper.Map(request, categoryToUpdate, typeof(UpdateCategoryCommand), typeof(Category));

        await categoryRepository.UpdateAsync(categoryToUpdate);

        return Unit.Value;
    }
}