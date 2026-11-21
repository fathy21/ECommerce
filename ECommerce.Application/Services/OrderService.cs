using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository,IRepository<Product> productRepository,IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponseDto> CreateAsync(string customerId,CreateOrderDto dto,CancellationToken cancellationToken = default)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            if (dto.Items.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
                throw new ArgumentException("Duplicate products are not allowed.");

            var order = new Order
            {
                CustomerId = customerId,
                OrderDateUtc = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0
            };

            var responseItems = new List<OrderItemResponseDto>();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                decimal totalAmount = 0;

                foreach (var item in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);

                    if (product is null)
                        throw new KeyNotFoundException(
                            $"Product {item.ProductId} was not found.");

                    if (product.StockQuantity < item.Quantity)
                        throw new InvalidOperationException(
                            $"Insufficient stock for product: {product.Name}");

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Product = product,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.Items.Add(orderItem);

                    product.StockQuantity -= item.Quantity;
                    _productRepository.Update(product);

                    var lineTotal = product.Price * item.Quantity;
                    totalAmount += lineTotal;

                    responseItems.Add(new OrderItemResponseDto
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        LineTotal = lineTotal
                    });
                }

                order.TotalAmount = totalAmount;

                await _orderRepository.AddAsync(order, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

            }, cancellationToken);

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDateUtc = order.OrderDateUtc,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = responseItems
            };
        }

        public async Task<IReadOnlyList<OrderResponseDto>> GetMyOrdersAsync(
     string customerId,
     CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(
                customerId, cancellationToken);

            return orders.Select(MapToResponse).ToList();
        }

        public async Task<OrderResponseDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(
                id, cancellationToken);

            return order is null ? null : MapToResponse(order);
        }

        public async Task<IReadOnlyList<OrderResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllWithItemsAsync(
                cancellationToken);

            return orders.Select(MapToResponse).ToList();
        }

        public async Task<bool> UpdateStatusAsync(
            Guid id,
            OrderStatus status,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(
                id, cancellationToken);

            if (order is null)
                return false;

            order.Status = status;

            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static OrderResponseDto MapToResponse(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDateUtc = order.OrderDateUtc,
                Status = order.Status,
                TotalAmount = order.TotalAmount,

                Items = order.Items.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                }).ToList()
            };
        }
    }
}
