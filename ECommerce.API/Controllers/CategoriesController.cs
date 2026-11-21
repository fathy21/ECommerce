using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(cancellationToken);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto,CancellationToken cancellationToken)
        {
            var category = await _categoryService.CreateAsync(dto , cancellationToken);
            return Ok(category);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);

            if (category is null)
                return NotFound();

            return Ok(category);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,UpdateCategoryDto dto,CancellationToken cancellationToken)
        {
            var category = await _categoryService.UpdateAsync(id, dto, cancellationToken);

            if (category is null)
                return NotFound();

            return Ok(category);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
