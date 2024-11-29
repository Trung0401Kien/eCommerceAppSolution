using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;

namespace eCommerceApp.Application.Services.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<GetCategory>> GetAllAsync();
        Task<GetCategory> GetByIdAsync(Guid id);
        Task<ServiceResponse> AddAsync(CreateCategory createCategory);
        Task<ServiceResponse> UpdateAsync(UpdateCategory updateCategory);
        Task<ServiceResponse> DeleteAsync(Guid id);
    }
}
