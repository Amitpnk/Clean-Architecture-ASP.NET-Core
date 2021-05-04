using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Exceptions;
using CleanArch.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArch.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IGenericRepositoryAsync<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(IMapper mapper, IGenericRepositoryAsync<Category> categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {

            var categoryToUpdate = await _categoryRepository.GetByIdAsync(request.CategoryId);

            if (categoryToUpdate == null)
            {
                throw new NotFoundException(nameof(Category), request.CategoryId);
            }

            var validator = new UpdateCategoryCommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            _mapper.Map(request, categoryToUpdate, typeof(UpdateCategoryCommand), typeof(Category));

            await _categoryRepository.UpdateAsync(categoryToUpdate);

            return Unit.Value;
        }
    }
}