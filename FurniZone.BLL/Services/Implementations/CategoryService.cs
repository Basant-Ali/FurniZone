using AutoMapper;
using FurniZone.BLL.ModelVM.Category;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<CategoryResponse>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var response = _mapper.Map<List<CategoryResponse>>(categories);
            return ApiResponse<List<CategoryResponse>>.SuccessResponse(response);
        }

        public async Task<ApiResponse<CategoryResponse>> GetByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return ApiResponse<CategoryResponse>.ErrorResponse("Category not found");
            }

            var response = _mapper.Map<CategoryResponse>(category);
            return ApiResponse<CategoryResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
        {
            var category = _mapper.Map<Category>(request);
            category.Id = Guid.NewGuid();
            category.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CategoryResponse>(category);
            return ApiResponse<CategoryResponse>.SuccessResponse(response, "Category created successfully");
        }

        public async Task<ApiResponse<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return ApiResponse<CategoryResponse>.ErrorResponse("Category not found");
            }

            category.Name = request.Name;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CategoryResponse>(category);
            return ApiResponse<CategoryResponse>.SuccessResponse(response, "Category updated successfully");
        }

        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return ApiResponse.ErrorResponse("Category not found");
            }

            // Check if category has products
            var categoryWithProducts = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id);
            if (categoryWithProducts?.Products?.Any() == true)
            {
                return ApiResponse.ErrorResponse("Cannot delete category with existing products");
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.SuccessResponse("Category deleted successfully");
        }
    }
}
