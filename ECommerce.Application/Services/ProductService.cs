using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService  //(contains business logic of product)
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IRepository<Product> ProductRepository,
            IRepository<Category> CategoryRepository, IRepository<Brand> BrandRepository , IUnitOfWork unitOfWork) 
        {
            _productRepository = ProductRepository;
            _categoryRepository = CategoryRepository;
            _brandRepository = BrandRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId , cancellationToken);
            if(category is null)
            {
                throw new ArgumentNullException("Category Not Found");
            }
            var brand = await _brandRepository.GetByIdAsync(dto.BrandId , cancellationToken);
            if(brand is null)
            {
                throw new ArgumentException("Brand Not Found");
            }
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity, 
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId
            };

            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToResponse(product);

        }
        public async Task<PaginatedResponseDto<ProductResponseDto>> GetAllAsync(ProductQueryParams queryParams,CancellationToken cancellationToken = default)
        {
            // Validate pagination
            var pageNumber = Math.Max(1, queryParams.PageNumber);
            var pageSize = Math.Clamp(queryParams.PageSize, 1, 100);

            // Start query
            var query = _productRepository.GetQueryable();

            // Search by product name
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search.Trim();
                query = query.Where(p => p.Name.Contains(search));
            }

            // Filter by Category
            if (queryParams.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == queryParams.CategoryId.Value);
            }

            // Filter by Brand
            if (queryParams.BrandId.HasValue)
            {
                query = query.Where(p =>
                    p.BrandId == queryParams.BrandId.Value);
            }

            // Sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "price" => queryParams.IsDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "name" => queryParams.IsDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                "createdat" => queryParams.IsDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),

                _ => query.OrderBy(p => p.Name)
            };

            // Pagination
            var (products, totalCount) =
                await _productRepository.GetPagedAsync(
                    query,
                    pageNumber,
                    pageSize,
                    cancellationToken);

            return new PaginatedResponseDto<ProductResponseDto>
            {
                Items = products.Select(MapToResponse).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        public async Task<ProductResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return null;
            return MapToResponse(product);
        }
        public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return null;

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);

            var brand = await _brandRepository.GetByIdAsync(dto.BrandId, cancellationToken);

            if (category is null)
                throw new ArgumentException("Category not found.");

            if (brand is null)
                throw new ArgumentException("Brand not found.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToResponse(product);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product is null)
                return false;

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        private static ProductResponseDto MapToResponse(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId
            };
        }

    }
}
