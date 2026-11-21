using ECommerce.Application.DTOs.Orders;
using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateAsync(string customerId,CreateOrderDto dto,CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OrderResponseDto>> GetMyOrdersAsync(string customerId,CancellationToken cancellationToken = default);

        Task<OrderResponseDto?> GetByIdAsync( Guid id,CancellationToken cancellationToken = default);

        Task<IReadOnlyList<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<bool> UpdateStatusAsync( Guid id,OrderStatus status,CancellationToken cancellationToken = default);
    }
}
