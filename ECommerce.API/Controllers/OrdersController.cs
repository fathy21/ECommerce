using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Customer: Create Order
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder(
            CreateOrderDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
                return Unauthorized();

            var result = await _orderService.CreateAsync(
                customerId, dto, cancellationToken);

            return StatusCode(201, result);
        }

        // Customer: Get My Orders
        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders(
            CancellationToken cancellationToken)
        {
            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
                return Unauthorized();

            var orders = await _orderService.GetMyOrdersAsync(
                customerId, cancellationToken);

            return Ok(orders);
        }

        // Customer or Admin: Get Order By ID
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetOrderById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var order = await _orderService.GetByIdAsync(
                id, cancellationToken);

            if (order is null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");

            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!isAdmin && order.CustomerId != customerId)
                return Forbid();

            return Ok(order);
        }

        // Admin: Get All Orders
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(
            CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllAsync(
                cancellationToken);

            return Ok(orders);
        }

        // Admin: Update Order Status
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(
            Guid id,
            UpdateOrderStatusDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _orderService.UpdateStatusAsync(
                id, dto.Status, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
