using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Wishlist?> GetWishlistByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wishlist?> GetWishlistWithItemsAsync(Guid wishlistId)
        {
            return await _context.Wishlists
                .AsNoTracking()
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.Id == wishlistId);
        }

        public async Task<Wishlist?> GetWishlistWithItemsByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}
