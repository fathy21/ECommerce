using ECommerce.Application.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IBrandService
    {
        Task<BrandResponseDto> CreateAsync(CreateBrandDto dto,CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BrandResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BrandResponseDto?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default);
        Task<BrandResponseDto?> UpdateAsync(Guid id,UpdateBrandDto dto,CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id,CancellationToken cancellationToken = default);
    }
}
