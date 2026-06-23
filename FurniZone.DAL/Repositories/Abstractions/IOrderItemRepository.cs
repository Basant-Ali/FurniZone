using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
    }
}
