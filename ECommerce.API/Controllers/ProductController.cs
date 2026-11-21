using ECommerce.Application.DTOs.Common;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.Authorization;
namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryParams queryParams, CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync( queryParams ,cancellationToken);
            return Ok(products);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id , CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if(product is null)
                return NotFound(new ApiResponseDto<object>(false,"Product not found.")); 

            return Ok(new ApiResponseDto<ProductResponseDto>(true,"Product retrieved successfully.",product));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto,CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById),new { id = product.Id },
                new ApiResponseDto<ProductResponseDto>(true,"Product created successfully.",
                product));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,UpdateProductDto dto,CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.UpdateAsync(id, dto, cancellationToken);
                if (product is null)
                    return NotFound();

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
