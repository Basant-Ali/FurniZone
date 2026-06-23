using AutoMapper;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.User;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserResponse>> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<UserResponse>.ErrorResponse("User not found");
            }

            var response = _mapper.Map<UserResponse>(user);
            return ApiResponse<UserResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<UserResponse>> GetByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return ApiResponse<UserResponse>.ErrorResponse("User not found");
            }

            var response = _mapper.Map<UserResponse>(user);
            return ApiResponse<UserResponse>.SuccessResponse(response);
        }
    }
}
