using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IWishlistItemRepository : IGenericRepository<WishlistItem>
    {
        Task<WishlistItem?> GetWishlistItemAsync(Guid wishlistId, Guid productId);
        Task<IEnumerable<WishlistItem>> GetWishlistItemsByWishlistIdAsync(Guid wishlistId);
    }
}
