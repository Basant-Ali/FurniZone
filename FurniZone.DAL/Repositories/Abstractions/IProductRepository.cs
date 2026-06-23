using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Models;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsAsync(ProductFilterParams filterParams);
        Task<int> CountAsync(ProductFilterParams filterParams);
        Task<Product?> GetProductWithDetailsAsync(Guid id);
    }
}
