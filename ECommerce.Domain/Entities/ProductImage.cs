using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}
