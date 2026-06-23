using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class WishlistItemRepository : GenericRepository<WishlistItem>, IWishlistItemRepository
    {
        public WishlistItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<WishlistItem?> GetWishlistItemAsync(Guid wishlistId, Guid productId)
        {
            return await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlistId && wi.ProductId == productId);
        }

        public async Task<IEnumerable<WishlistItem>> GetWishlistItemsByWishlistIdAsync(Guid wishlistId)
        {
            return await _context.WishlistItems
                .AsNoTracking()
                .Where(wi => wi.WishlistId == wishlistId)
                .Include(wi => wi.Product)
                .ToListAsync();
        }
    }
}
