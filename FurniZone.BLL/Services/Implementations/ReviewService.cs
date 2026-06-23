using AutoMapper;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Review;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<ReviewResponse>>> GetProductReviewsAsync(Guid productId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByProductIdAsync(productId);
            var response = _mapper.Map<List<ReviewResponse>>(reviews);
            return ApiResponse<List<ReviewResponse>>.SuccessResponse(response);
        }

        public async Task<ApiResponse<List<ReviewResponse>>> GetUserReviewsAsync(Guid userId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByUserIdAsync(userId);
            var response = _mapper.Map<List<ReviewResponse>>(reviews);
            return ApiResponse<List<ReviewResponse>>.SuccessResponse(response);
        }

        public async Task<ApiResponse<ReviewResponse>> CreateAsync(Guid userId, CreateReviewRequest request)
        {
            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return ApiResponse<ReviewResponse>.ErrorResponse("Product not found");
            }

            // Check if user already reviewed this product
            var existingReview = await _unitOfWork.Reviews.GetReviewByUserAndProductAsync(userId, request.ProductId);
            if (existingReview != null)
            {
                return ApiResponse<ReviewResponse>.ErrorResponse("You have already reviewed this product");
            }

            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return ApiResponse<ReviewResponse>.ErrorResponse("Rating must be between 1 and 5");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Load user for response
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            review.User = user!;

            var response = _mapper.Map<ReviewResponse>(review);
            return ApiResponse<ReviewResponse>.SuccessResponse(response, "Review added successfully");
        }

        public async Task<ApiResponse<ReviewResponse>> UpdateAsync(Guid userId, Guid reviewId, UpdateReviewRequest request)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                return ApiResponse<ReviewResponse>.ErrorResponse("Review not found");
            }

            // Check ownership
            if (review.UserId != userId)
            {
                return ApiResponse<ReviewResponse>.ErrorResponse("You can only update your own reviews");
            }

            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return ApiResponse<ReviewResponse>.ErrorResponse("Rating must be between 1 and 5");
            }

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            review.User = user!;

            var response = _mapper.Map<ReviewResponse>(review);
            return ApiResponse<ReviewResponse>.SuccessResponse(response, "Review updated successfully");
        }

        public async Task<ApiResponse> DeleteAsync(Guid userId, Guid reviewId, bool isAdmin = false)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                return ApiResponse.ErrorResponse("Review not found");
            }

            // Check ownership or admin
            if (review.UserId != userId && !isAdmin)
            {
                return ApiResponse.ErrorResponse("You can only delete your own reviews");
            }

            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.SuccessResponse("Review deleted successfully");
        }
    }
}
