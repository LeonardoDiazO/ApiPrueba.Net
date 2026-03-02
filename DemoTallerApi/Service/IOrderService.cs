using DemoTallerApi.DTOs;

namespace DemoTallerApi.Service
{
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<PaginatedResponseDto<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize);
        Task<PaginatedResponseDto<OrderDto>> GetOrdersByUserIdAsync(int userId, int pageNumber, int pageSize);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderStatusAsync(int id, string status);
        Task DeleteOrderAsync(int id);
    }
}
