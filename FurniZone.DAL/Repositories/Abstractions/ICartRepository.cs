using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<Cart?> GetCartWithItemsAsync(Guid cartId);
        Task<Cart?> GetCartWithItemsByUserIdAsync(Guid userId);
    }
}
