using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : ICommand<CreateCategoryCommandResponse>
{
    public string Name { get; set; }
}