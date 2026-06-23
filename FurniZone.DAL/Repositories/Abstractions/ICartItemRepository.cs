using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId);
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(Guid cartId);
    }
}
