using FurniZone.DAL.Enums;

namespace FurniZone.BLL.ModelVM.Auth
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
