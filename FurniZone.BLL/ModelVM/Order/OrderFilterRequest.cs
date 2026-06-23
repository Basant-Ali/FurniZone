using FurniZone.DAL.Enums;

namespace FurniZone.BLL.ModelVM.Order
{
    public class OrderFilterRequest
    {
        public OrderStatus? Status { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
