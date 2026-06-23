using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
        {
            return await _context.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
