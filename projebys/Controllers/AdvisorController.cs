using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projebys.Data;

[Route("api/[controller]")]
[ApiController]
public class AdvisorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdvisorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("getAdvisorInfo")]
    public IActionResult GetAdvisorInfo()
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null)
        {
            return Unauthorized(new { message = "Kullanıcı giriş yapmamış." });
        }

        // Kullanıcı ve danışman bilgilerini al
        var user = _context.Users.Include(u => u.Advisor).FirstOrDefault(u => u.UserID == userId);
        if (user == null || user.Advisor == null)
        {
            return NotFound(new { message = "Kullanıcı veya danışman bilgileri bulunamadı." });
        }

        return Ok(new
        {
            user.UserID,
            user.Username,
            user.Email,
            advisorInfo = new
            {
                user.Advisor.AdvisorID,
                user.Advisor.FullName,
                user.Advisor.Department,
                user.Advisor.Title
            }
        });
    }

}
