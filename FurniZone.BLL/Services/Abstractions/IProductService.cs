using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Product;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IProductService
    {
        Task<ApiResponse<PagedResponse<ProductResponse>>> GetAllAsync(ProductFilterRequest request);
        Task<ApiResponse<ProductDetailResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<ProductResponse>> CreateAsync(CreateProductRequest request);
        Task<ApiResponse<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request);
        Task<ApiResponse<ProductResponse>> PatchAsync(Guid id, PatchProductRequest request);
        Task<ApiResponse> DeleteAsync(Guid id);
    }
}
