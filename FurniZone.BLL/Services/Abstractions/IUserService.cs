using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.User;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<UserResponse>> GetByEmailAsync(string email);
    }
}
