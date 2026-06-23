namespace FurniZone.BLL.ModelVM.Wishlist
{
    public class WishlistItemResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
