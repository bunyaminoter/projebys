using System.ComponentModel.DataAnnotations;

namespace projebys.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }  // Kullanıcı kimliği
        public string Username { get; set; } // Kullanıcı adı
        public string PasswordHash { get; set; } // Şifre hash'i
        public string Role { get; set; }  // Kullanıcı rolü (örneğin, "Advisor" veya "Student")
        public int RelatedID { get; set; } // İlişkili kimlik (örneğin, danışman veya öğrenci ID'si)
        public string Email { get; set; } // Kullanıcının e-posta adresi

        public Students Student { get; set; }
        public Advisors Advisor { get; set; }
    }
}
