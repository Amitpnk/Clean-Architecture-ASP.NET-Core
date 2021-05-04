using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArch.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IGenericRepositoryAsync<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public DeleteCategoryCommandHandler(IMapper mapper, IGenericRepositoryAsync<Category> categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryToDelete = await _categoryRepository.GetByIdAsync(request.CategoryId);

            if (categoryToDelete == null)
            {
                throw new NotFoundException(nameof(Category), request.CategoryId);
            }

            await _categoryRepository.DeleteAsync(categoryToDelete);

            return Unit.Value;
        }
    }
}
