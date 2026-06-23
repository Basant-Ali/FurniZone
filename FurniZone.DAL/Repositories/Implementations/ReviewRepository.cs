using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<double> GetAverageRatingByProductIdAsync(Guid productId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }
    }
}
