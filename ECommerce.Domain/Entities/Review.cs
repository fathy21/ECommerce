using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;

        public Guid ProductId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public Product Product { get; set; } = null!;   
    }
}
