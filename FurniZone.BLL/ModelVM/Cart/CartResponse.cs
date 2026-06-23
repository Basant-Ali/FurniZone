namespace FurniZone.BLL.ModelVM.Cart
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
    }
}
