using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;

        public DateTime OrderDateUtc { get; set; }

        public OrderStatus Status { get; set; }  //enum

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}
