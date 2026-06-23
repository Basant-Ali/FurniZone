using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);
        Task<Review?> GetReviewByUserAndProductAsync(Guid userId, Guid productId);
        Task<double> GetAverageRatingByProductIdAsync(Guid productId);
    }
}
