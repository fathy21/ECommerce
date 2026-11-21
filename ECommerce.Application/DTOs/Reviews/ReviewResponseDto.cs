using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Reviews
{
    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
