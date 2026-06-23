using FurniZone.BLL.ModelVM.Review;

namespace FurniZone.BLL.ModelVM.Product
{
    public class ProductDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
