using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Review;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IReviewService
    {
        Task<ApiResponse<List<ReviewResponse>>> GetProductReviewsAsync(Guid productId);
        Task<ApiResponse<List<ReviewResponse>>> GetUserReviewsAsync(Guid userId);
        Task<ApiResponse<ReviewResponse>> CreateAsync(Guid userId, CreateReviewRequest request);
        Task<ApiResponse<ReviewResponse>> UpdateAsync(Guid userId, Guid reviewId, UpdateReviewRequest request);
        Task<ApiResponse> DeleteAsync(Guid userId, Guid reviewId, bool isAdmin = false);
    }
}
