namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IWishlistRepository Wishlists { get; }
        IWishlistItemRepository WishlistItems { get; }
        IReviewRepository Reviews { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IPaymentRepository Payments { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}
