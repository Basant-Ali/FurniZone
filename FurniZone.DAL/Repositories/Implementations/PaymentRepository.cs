using FurniZone.DAL.Database;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Repositories.Implementations
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId)
        {
            return await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }
    }
}
