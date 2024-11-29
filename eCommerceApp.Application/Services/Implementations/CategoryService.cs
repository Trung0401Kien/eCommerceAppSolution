using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.Services.Interface;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementations
{
    public class CategoryService(IGeneric<Category> categoryInterface, IMapper mapper) : ICategoryService
    {
        public async Task<ServiceResponse> AddAsync(CreateCategory createCategory)
        {
            var mapping = mapper.Map<Category>(createCategory);
            int result = await categoryInterface.AddAsync(mapping);
            return result > 0 ? new ServiceResponse(true, "Category added!") :
                new ServiceResponse(false, "Category failded to be deleted");
        }

        public async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            int result = await categoryInterface.DeleteAsync(id);
            return result > 0 ? new ServiceResponse(true, "Category deleted!") :
                new ServiceResponse(false, "Category not found or failded to be deleted");
        }

        public async Task<IEnumerable<GetCategory>> GetAllAsync()
        {
            var rawData = await categoryInterface.GetAllAsync();
            if (!rawData.Any()) return [];

            return mapper.Map<IEnumerable<GetCategory>>(rawData);
        }

        public async Task<GetCategory> GetByIdAsync(Guid id)
        {
            var rawData = await categoryInterface.GetByIdAsync(id);
            if (rawData == null) return new GetCategory();

            return mapper.Map<GetCategory>(rawData);
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateCategory updateCategory)
        {
            var mapping = mapper.Map<Category>(updateCategory);
            int result = await categoryInterface.UpdateAsync(mapping);
            return result > 0 ? new ServiceResponse(true, "Category updated!") :
                new ServiceResponse(false, "Category failded to be updated");
        }
    }
}
