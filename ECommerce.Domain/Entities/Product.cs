using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }  
        public int StockQuantity { get; set; }
        // Foreign Keys
        public Guid CategoryId { get; set; }    
        public Guid BrandId { get; set; }
        // Navigation Properties
        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; }
           = new List<ProductImage>();

        public ICollection<Review> Reviews { get; set; }
            = new List<Review>();
    }
}
