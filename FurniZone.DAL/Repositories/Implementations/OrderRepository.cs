using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using FurniZone.DAL.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(OrderFilterParams filterParams)
        {
            var query = _context.Orders.AsQueryable();

            // Apply filters
            if (filterParams.Status.HasValue)
                query = query.Where(o => o.Status == filterParams.Status.Value);

            if (filterParams.UserId.HasValue)
                query = query.Where(o => o.UserId == filterParams.UserId.Value);

            if (filterParams.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= filterParams.FromDate.Value);

            if (filterParams.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= filterParams.ToDate.Value);

            // Apply sorting
            query = filterParams.SortBy?.ToLower() switch
            {
                "totalprice" => filterParams.SortDescending
                    ? query.OrderByDescending(o => o.TotalPrice)
                    : query.OrderBy(o => o.TotalPrice),
                "status" => filterParams.SortDescending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),
                _ => filterParams.SortDescending
                    ? query.OrderByDescending(o => o.CreatedAt)
                    : query.OrderBy(o => o.CreatedAt)
            };

            // Apply pagination
            query = query.Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                        .Take(filterParams.PageSize);

            return await query
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<int> CountAllAsync(OrderFilterParams filterParams)
        {
            var query = _context.Orders.AsQueryable();

            if (filterParams.Status.HasValue)
                query = query.Where(o => o.Status == filterParams.Status.Value);

            if (filterParams.UserId.HasValue)
                query = query.Where(o => o.UserId == filterParams.UserId.Value);

            if (filterParams.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= filterParams.FromDate.Value);

            if (filterParams.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= filterParams.ToDate.Value);

            return await query.CountAsync();
        }
    }
}
