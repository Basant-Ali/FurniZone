using FurniZone.BLL.ModelVM.Auth;
using FurniZone.BLL.ModelVM.Common;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> SignUpAsync(SignUpRequest request);
        Task<ApiResponse<AuthResponse>> SignInAsync(SignInRequest request);
        Task<ApiResponse> LogoutAsync(Guid userId);
        Task<ApiResponse<AuthResponse>> CreateAdminAsync(SignUpRequest request);
    }
}
