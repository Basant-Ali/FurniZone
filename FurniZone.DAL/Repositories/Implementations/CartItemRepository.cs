using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(Guid cartId)
        {
            return await _context.CartItems
                .AsNoTracking()
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product)
                .ToListAsync();
        }
    }
}
