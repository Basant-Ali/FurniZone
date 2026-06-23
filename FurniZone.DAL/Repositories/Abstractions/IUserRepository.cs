using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailWithDetailsAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserNameExistsAsync(string userName);
    }
}
