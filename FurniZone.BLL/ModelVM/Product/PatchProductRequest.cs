namespace FurniZone.BLL.ModelVM.Product
{
    public class PatchProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
