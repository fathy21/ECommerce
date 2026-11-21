using ECommerce.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateAsync (CreateProductDto createProductDto , CancellationToken cancellationToken=default);
        Task<PaginatedResponseDto<ProductResponseDto>> GetAllAsync(ProductQueryParams queryParams,CancellationToken cancellationToken = default);
        Task<ProductResponseDto?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default);
        Task<ProductResponseDto?> UpdateAsync(Guid id,UpdateProductDto dto,CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id,CancellationToken cancellationToken = default);
    }
}