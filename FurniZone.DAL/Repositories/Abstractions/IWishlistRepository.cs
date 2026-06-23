using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IWishlistRepository : IGenericRepository<Wishlist>
    {
        Task<Wishlist?> GetWishlistByUserIdAsync(Guid userId);
        Task<Wishlist?> GetWishlistWithItemsAsync(Guid wishlistId);
        Task<Wishlist?> GetWishlistWithItemsByUserIdAsync(Guid userId);
    }
}
