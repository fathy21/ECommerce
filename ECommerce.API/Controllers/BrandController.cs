using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetAllAsync(cancellationToken);
            return Ok(brands);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDto dto,CancellationToken cancellationToken)
        {
            var brand = await _brandService.CreateAsync(dto, cancellationToken);
            return Ok(brand);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
        {
            var brand = await _brandService.GetByIdAsync(id, cancellationToken);

            if (brand is null)
                return NotFound();

            return Ok(brand);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,UpdateBrandDto dto,CancellationToken cancellationToken)
        {
            var brand = await _brandService.UpdateAsync(id, dto, cancellationToken);

            if (brand is null)
                return NotFound();

            return Ok(brand);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete( Guid id,CancellationToken cancellationToken)
        {
            var deleted = await _brandService.DeleteAsync( id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
