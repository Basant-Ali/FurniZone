using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Models;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order?> GetOrderWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync(OrderFilterParams filterParams);
        Task<int> CountAllAsync(OrderFilterParams filterParams);
    }
}
