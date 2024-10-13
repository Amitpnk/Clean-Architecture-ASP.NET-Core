using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using MediatR;

namespace CleanArch.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    IMapper mapper,
    ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, CreateCategoryCommandResponse>
{
    public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var createCategoryCommandResponse = new CreateCategoryCommandResponse();

        var validator = new CreateCategoryCommandValidator(categoryRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Count > 0)
        {
            createCategoryCommandResponse.Success = false;
            createCategoryCommandResponse.ValidationErrors = new List<string>();
            foreach (var error in validationResult.Errors)
            {
                createCategoryCommandResponse.ValidationErrors.Add(error.ErrorMessage);
            }
        }
        if (createCategoryCommandResponse.Success)
        {
            var category = new Category() { Name = request.Name };
            category = await categoryRepository.AddAsync(category);
            createCategoryCommandResponse.Category = mapper.Map<CreateCategoryDto>(category);
        }

        return createCategoryCommandResponse;
    }
}