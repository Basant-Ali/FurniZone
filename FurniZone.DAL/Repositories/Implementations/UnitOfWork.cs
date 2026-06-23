using FurniZone.DAL.Database;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public ICartRepository Carts { get; }
        public ICartItemRepository CartItems { get; }
        public IWishlistRepository Wishlists { get; }
        public IWishlistItemRepository WishlistItems { get; }
        public IReviewRepository Reviews { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IPaymentRepository Payments { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new ProductRepository(context);
            Categories = new CategoryRepository(context);
            Carts = new CartRepository(context);
            CartItems = new CartItemRepository(context);
            Wishlists = new WishlistRepository(context);
            WishlistItems = new WishlistItemRepository(context);
            Reviews = new ReviewRepository(context);
            Orders = new OrderRepository(context);
            OrderItems = new OrderItemRepository(context);
            Payments = new PaymentRepository(context);
            Users = new UserRepository(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
