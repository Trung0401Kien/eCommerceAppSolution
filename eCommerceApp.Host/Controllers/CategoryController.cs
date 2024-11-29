using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {

        [HttpGet("get-all-category")]
        public async Task<IActionResult> GetAll()
        {
            var result = await categoryService.GetAllAsync();
            return result.Any() ? Ok(result) : NotFound(result);
        }

        [HttpGet("single-{id}")]
        public async Task<IActionResult> GetSingle(Guid id)
        {
            var result = await categoryService.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory(CreateCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await categoryService.AddAsync(category);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-category")]
        public async Task<IActionResult> UpdateCategory(UpdateCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await categoryService.UpdateAsync(category);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-category-{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await categoryService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
