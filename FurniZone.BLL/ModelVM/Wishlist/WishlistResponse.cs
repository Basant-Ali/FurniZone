namespace FurniZone.BLL.ModelVM.Wishlist
{
    public class WishlistResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<WishlistItemResponse> Items { get; set; } = new();
        public int TotalItems { get; set; }
    }
}
