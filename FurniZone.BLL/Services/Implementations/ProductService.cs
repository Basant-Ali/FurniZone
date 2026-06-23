using AutoMapper;
using FurniZone.BLL.Helpers;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Product;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;
using FurniZone.DAL.Repositories.Models;

namespace FurniZone.BLL.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileUploadHelper _fileUploadHelper;
        private readonly IPaginationHelper _paginationHelper;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileUploadHelper fileUploadHelper,
            IPaginationHelper paginationHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileUploadHelper = fileUploadHelper;
            _paginationHelper = paginationHelper;
        }

        public async Task<ApiResponse<PagedResponse<ProductResponse>>> GetAllAsync(ProductFilterRequest request)
        {
            var filterParams = new ProductFilterParams
            {
                CategoryId = request.CategoryId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                SearchTerm = request.SearchTerm,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var products = await _unitOfWork.Products.GetProductsAsync(filterParams);
            var totalCount = await _unitOfWork.Products.CountAsync(filterParams);

            var productResponses = _mapper.Map<List<ProductResponse>>(products);

            // Calculate average rating for each product
            foreach (var product in productResponses)
            {
                product.AverageRating = await _unitOfWork.Reviews.GetAverageRatingByProductIdAsync(product.Id);
            }

            var pagedResponse = _paginationHelper.CreatePagedResponse(
                productResponses, request.PageNumber, request.PageSize, totalCount);

            return ApiResponse<PagedResponse<ProductResponse>>.SuccessResponse(pagedResponse);
        }

        public async Task<ApiResponse<ProductDetailResponse>> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id);
            if (product == null)
            {
                return ApiResponse<ProductDetailResponse>.ErrorResponse("Product not found");
            }

            var response = _mapper.Map<ProductDetailResponse>(product);
            response.AverageRating = await _unitOfWork.Reviews.GetAverageRatingByProductIdAsync(id);

            return ApiResponse<ProductDetailResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<ProductResponse>> CreateAsync(CreateProductRequest request)
        {
            // Check if category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<ProductResponse>.ErrorResponse("Category not found");
            }

            var product = _mapper.Map<Product>(request);
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;

            // Handle image upload
            if (request.Image != null)
            {
                product.ImageUrl = await _fileUploadHelper.UploadImageAsync(request.Image, "products");
            }
            else
            {
                product.ImageUrl = "/images/products/default.jpg";
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponse>(product);
            return ApiResponse<ProductResponse>.SuccessResponse(response, "Product created successfully");
        }

        public async Task<ApiResponse<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return ApiResponse<ProductResponse>.ErrorResponse("Product not found");
            }

            // Check if category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<ProductResponse>.ErrorResponse("Category not found");
            }

            // Handle image upload
            if (request.Image != null)
            {
                // Delete old image if not default
                if (!product.ImageUrl.Contains("default"))
                {
                    _fileUploadHelper.DeleteImage(product.ImageUrl);
                }
                product.ImageUrl = await _fileUploadHelper.UploadImageAsync(request.Image, "products");
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponse>(product);
            return ApiResponse<ProductResponse>.SuccessResponse(response, "Product updated successfully");
        }

        public async Task<ApiResponse<ProductResponse>> PatchAsync(Guid id, PatchProductRequest request)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return ApiResponse<ProductResponse>.ErrorResponse("Product not found");
            }

            if (request.Name != null)
                product.Name = request.Name;

            if (request.Description != null)
                product.Description = request.Description;

            if (request.Price.HasValue)
                product.Price = request.Price.Value;

            if (request.Stock.HasValue)
                product.Stock = request.Stock.Value;

            if (request.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                {
                    return ApiResponse<ProductResponse>.ErrorResponse("Category not found");
                }
                product.CategoryId = request.CategoryId.Value;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponse>(product);
            return ApiResponse<ProductResponse>.SuccessResponse(response, "Product updated successfully");
        }

        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return ApiResponse.ErrorResponse("Product not found");
            }

            // Delete image if not default
            if (!product.ImageUrl.Contains("default"))
            {
                _fileUploadHelper.DeleteImage(product.ImageUrl);
            }

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.SuccessResponse("Product deleted successfully");
        }
    }
}
