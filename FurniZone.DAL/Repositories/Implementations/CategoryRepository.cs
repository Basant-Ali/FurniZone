using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithProductsAsync(Guid id)
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
