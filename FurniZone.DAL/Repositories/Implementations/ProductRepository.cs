using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using FurniZone.DAL.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(ProductFilterParams filterParams)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            // Apply filters
            if (filterParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filterParams.CategoryId.Value);

            if (filterParams.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filterParams.MinPrice.Value);

            if (filterParams.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filterParams.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
                query = query.Where(p => p.Name.Contains(filterParams.SearchTerm) ||
                                          p.Description.Contains(filterParams.SearchTerm));

            // Apply sorting
            query = filterParams.SortBy?.ToLower() switch
            {
                "price" => filterParams.SortDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "name" => filterParams.SortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "date" => filterParams.SortDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Apply pagination
            query = query.Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                        .Take(filterParams.PageSize);

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(ProductFilterParams filterParams)
        {
            var query = _context.Products.AsQueryable();

            if (filterParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filterParams.CategoryId.Value);

            if (filterParams.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filterParams.MinPrice.Value);

            if (filterParams.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filterParams.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
                query = query.Where(p => p.Name.Contains(filterParams.SearchTerm) ||
                                          p.Description.Contains(filterParams.SearchTerm));

            return await query.CountAsync();
        }

        public async Task<Product?> GetProductWithDetailsAsync(Guid id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
