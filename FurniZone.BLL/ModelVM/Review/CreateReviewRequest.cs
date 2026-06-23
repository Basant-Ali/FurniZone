namespace FurniZone.BLL.ModelVM.Review
{
    public class CreateReviewRequest
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
