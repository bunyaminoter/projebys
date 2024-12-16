
namespace projebys.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } // E-posta adresi
        public string PasswordHash { get; set; } // Şifre
        public string SelectedRole { get; set; }
    }
}