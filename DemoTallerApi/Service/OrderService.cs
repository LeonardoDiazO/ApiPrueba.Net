using AutoMapper;
using DemoTallerApi.Data;
using DemoTallerApi.DTOs;
using DemoTallerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTallerApi.Service
{
    public class OrderService : IOrderService
    {
        private readonly StoreDBContext _context;
        private readonly IMapper _mapper;

        public OrderService(StoreDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<PaginatedResponseDto<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Orders.CountAsync();

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderDto>>(orders);

            return new PaginatedResponseDto<OrderDto>
            {
                Items = orderDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponseDto<OrderDto>> GetOrdersByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            var totalCount = await _context.Orders.Where(o => o.UserId == userId).CountAsync();

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderDto>>(orders);

            return new PaginatedResponseDto<OrderDto>
            {
                Items = orderDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Validate user exists
            var user = await _context.Users.FindAsync(createOrderDto.UserId);
            if (user == null || !user.IsActive)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Create order
            var order = new OrderEntity
            {
                UserId = createOrderDto.UserId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItemEntity>();

            // Process order items
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null || !product.IsActive)
                {
                    throw new KeyNotFoundException($"Producto con ID {itemDto.ProductId} no encontrado");
                }

                if (product.Stock < itemDto.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");
                }

                var orderItem = new OrderItemEntity
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                totalAmount += orderItem.UnitPrice * orderItem.Quantity;
                orderItems.Add(orderItem);

                // Update product stock
                product.Stock -= itemDto.Quantity;
                product.UpdatedDate = DateTime.Now;
            }

            order.TotalAmount = totalAmount;
            order.OrderItems = orderItems;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reload with includes for mapping
            var createdOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstAsync(o => o.Id == order.Id);

            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException("Orden no encontrada");
            }

            var validStatuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException("Estado inválido");
            }

            order.Status = status;
            order.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException("Orden no encontrada");
            }

            // Only allow deletion of pending orders
            if (order.Status != "Pending")
            {
                throw new InvalidOperationException("Solo se pueden eliminar órdenes pendientes");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
