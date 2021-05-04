using MediatR;
using System;

namespace CleanArch.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest
    {
        public Guid CategoryId { get; set; }
    }
}
