using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : ICommand
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
}