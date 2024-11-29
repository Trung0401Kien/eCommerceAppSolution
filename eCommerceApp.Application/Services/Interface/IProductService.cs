using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;

namespace eCommerceApp.Application.Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<GetProduct>> GetAllAsync();
        Task<GetProduct> GetByIdAsync(Guid id);
        Task<ServiceResponse> AddAsync(CreateProduct createProduct);
        Task<ServiceResponse> UpdateAsync(UpdateProduct updateProduct);
        Task<ServiceResponse> DeleteAsync(Guid id);
    }
}
