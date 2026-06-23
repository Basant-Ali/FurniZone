using FurniZone.DAL.Entities;

namespace FurniZone.DAL.Repositories.Abstractions
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId);
    }
}
