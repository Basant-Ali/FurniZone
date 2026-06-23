using FurniZone.BLL.ModelVM.Category;
using FurniZone.BLL.ModelVM.Common;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryResponse>>> GetAllAsync();
        Task<ApiResponse<CategoryResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<CategoryResponse>> CreateAsync(CreateCategoryRequest request);
        Task<ApiResponse<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request);
        Task<ApiResponse> DeleteAsync(Guid id);
    }
}
