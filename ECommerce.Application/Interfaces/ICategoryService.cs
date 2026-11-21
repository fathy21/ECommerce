using ECommerce.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto,CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryResponseDto?> GetByIdAsync (Guid id,CancellationToken cancellationToken = default);
        Task<CategoryResponseDto?> UpdateAsync(Guid id,UpdateCategoryDto dto,CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id , CancellationToken cancellationToken = default);
    }
}
