using ECommerce.Application.DTOs.Brand;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IRepository<Brand> _brandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IRepository<Brand> brandRepository,IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BrandResponseDto> CreateAsync(CreateBrandDto dto,CancellationToken cancellationToken = default)
        {
            var brand = new Brand
            {
                Name = dto.Name
            };

            await _brandRepository.AddAsync(
                brand, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BrandResponseDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<IReadOnlyList<BrandResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var brands = await _brandRepository.GetAllAsync(cancellationToken);

            return brands.Select(brand => new BrandResponseDto
            {
                Id = brand.Id,
                Name = brand.Name
            }).ToList();
        }
        public async Task<BrandResponseDto?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(id, cancellationToken);

            if (brand is null)
                return null;

            return new BrandResponseDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }
        public async Task<BrandResponseDto?> UpdateAsync(Guid id,UpdateBrandDto dto,CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(id, cancellationToken);

            if (brand is null)
                return null;

            brand.Name = dto.Name;

            _brandRepository.Update(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BrandResponseDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }
        public async Task<bool> DeleteAsync(Guid id,CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository.GetByIdAsync(id, cancellationToken);

            if (brand is null)
                return false;

            _brandRepository.Delete(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
