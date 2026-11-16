using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : ICommand
{
    public Guid CategoryId { get; set; }
}