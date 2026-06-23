using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetCategoryWithProductsAsync(Guid id);
    }
}
