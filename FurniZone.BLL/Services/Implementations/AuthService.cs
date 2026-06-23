using AutoMapper;
using FurniZone.BLL.Helpers;
using FurniZone.BLL.ModelVM.Auth;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Enums;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtHelper _jwtHelper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtHelper jwtHelper,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtHelper = jwtHelper;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AuthResponse>> SignUpAsync(SignUpRequest request)
        {
            // Check if email exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Email already exists");
            }

            // Check if username exists
            if (await _unitOfWork.Users.UserNameExistsAsync(request.UserName))
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Username already exists");
            }

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = UserRole.User, // Always create as User
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);

            // Create cart and wishlist for user
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            var wishlist = new Wishlist
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.Wishlists.AddAsync(wishlist);

            await _unitOfWork.SaveChangesAsync();

            // Generate token
            var token = _jwtHelper.GenerateToken(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Token = token;
            response.ExpiresAt = DateTime.UtcNow.AddHours(24);

            return ApiResponse<AuthResponse>.SuccessResponse(response, "User registered successfully");
        }

        public async Task<ApiResponse<AuthResponse>> SignInAsync(SignInRequest request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Invalid credentials");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Invalid credentials");
            }

            var token = _jwtHelper.GenerateToken(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Token = token;
            response.ExpiresAt = DateTime.UtcNow.AddHours(24);

            return ApiResponse<AuthResponse>.SuccessResponse(response, "Login successful");
        }

        public Task<ApiResponse> LogoutAsync(Guid userId)
        {
            // In a real implementation, you might invalidate the token or add it to a blacklist
            // For this simulation, we just return success
            return Task.FromResult(ApiResponse.SuccessResponse("Logout successful"));
        }

        public async Task<ApiResponse<AuthResponse>> CreateAdminAsync(SignUpRequest request)
        {
            // Check if email exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Email already exists");
            }

            // Check if username exists
            if (await _unitOfWork.Users.UserNameExistsAsync(request.UserName))
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Username already exists");
            }

            // Create admin user (only difference is the Role)
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = UserRole.Admin,  // <-- Admin role
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);

            // Create cart and wishlist for admin
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            var wishlist = new Wishlist
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.Wishlists.AddAsync(wishlist);

            await _unitOfWork.SaveChangesAsync();

            // Generate token
            var token = _jwtHelper.GenerateToken(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Token = token;
            response.ExpiresAt = DateTime.UtcNow.AddHours(24);

            return ApiResponse<AuthResponse>.SuccessResponse(response, "Admin user created successfully");
        }
    }
}
