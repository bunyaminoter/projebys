using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projebys.Data;
using projebys.Models;

namespace projebys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Giriş işlemi (LoginController)
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.PasswordHash) || string.IsNullOrEmpty(request.SelectedRole))
            {
                return BadRequest(new { message = "E-posta, şifre ve rol bilgisi zorunludur." });
            }

            var expectedRole = request.SelectedRole.Equals("Personnel", StringComparison.OrdinalIgnoreCase) ? "Advisor" : request.SelectedRole;

            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.PasswordHash);

            if (user == null)
            {
                return Unauthorized(new { message = "E-posta veya şifre yanlış." });
            }

            if (!string.Equals(user.Role, expectedRole, StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, new { message = "Lütfen kendi alanınızdan giriş yapınız." });
            }

            // Kullanıcı bilgilerini oturumda tutma (Session)
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Giriş başarılı
            return Ok(new
            {
                message = "Giriş başarılı.",
                user = new
                {
                    user.UserID,
                    user.Username,
                    user.Email,
                    user.Role
                }
            });
        }


        // Logout API
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Oturum verilerini temizle (örneğin, JWT veya session bilgilerini)
            HttpContext.Session.Clear(); // Eğer session kullanıyorsanız
            return Ok(new { success = true, message = "Oturum başarıyla kapatıldı." });
        }

    }
}
