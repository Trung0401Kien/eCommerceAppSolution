using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interface;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementations
{
    public class ProductService(IGeneric<Product> productInterface, IMapper mapper) : IProductService
    {
        public async Task<ServiceResponse> AddAsync(CreateProduct createProduct)
        {
            var mapping = mapper.Map<Product>(createProduct);
            int result = await productInterface.AddAsync(mapping);
            return result > 0 ? new ServiceResponse(true, "Product added!") :
                new ServiceResponse(false, "Product failded to be deleted");
        }

        public async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            int result = await productInterface.DeleteAsync(id);
            return result > 0 ? new ServiceResponse(true, "Product deleted!") :
                new ServiceResponse(false, "Product failded to be deleted");
        }

        public async Task<IEnumerable<GetProduct>> GetAllAsync()
        {
            var rawData = await productInterface.GetAllAsync();
            if (!rawData.Any()) return [];

            return mapper.Map<IEnumerable<GetProduct>>(rawData);
        }

        public async Task<GetProduct> GetByIdAsync(Guid id)
        {
            var rawData = await productInterface.GetByIdAsync(id);
            if (rawData == null) return new GetProduct();

            return mapper.Map<GetProduct>(rawData);
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateProduct updateProduct)
        {
            var mapping = mapper.Map<Product>(updateProduct);
            int result = await productInterface.UpdateAsync(mapping);
            return result > 0 ? new ServiceResponse(true, "Product updated!") :
                new ServiceResponse(false, "Product failded to be updated");
        }
    }
}
