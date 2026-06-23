using Microsoft.AspNetCore.Http;

namespace FurniZone.BLL.ModelVM.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
