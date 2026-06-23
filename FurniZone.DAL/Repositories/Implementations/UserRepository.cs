using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByEmailWithDetailsAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Wishlist)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}
