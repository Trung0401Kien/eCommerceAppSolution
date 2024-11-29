using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {

        [HttpGet("get-all-product")]
        public async Task<IActionResult> GetAll()
        {
            var result = await productService.GetAllAsync();
            return result.Any() ? Ok(result) : NotFound(result);
        }

        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetSingle(Guid id)
        {
            var result = await productService.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(CreateProduct product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await productService.AddAsync(product);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(UpdateProduct product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await productService.UpdateAsync(product);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-product-{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await productService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
