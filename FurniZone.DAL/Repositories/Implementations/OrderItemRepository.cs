using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderItems
                .AsNoTracking()
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ToListAsync();
        }
    }
}
