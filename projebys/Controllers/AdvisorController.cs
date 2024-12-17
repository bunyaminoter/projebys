using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projebys.Data;

namespace projebys.Controllers
{

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

    // Danışmanın öğrencilerini getir
    [HttpGet("GetMyStudents")]
    public async Task<IActionResult> GetMyStudents()
    {
        var advisorId = HttpContext.Session.GetInt32("UserID"); // Oturumdaki danışman ID
        if (advisorId == null)
        {
            return Unauthorized(new { message = "Kullanıcı giriş yapmamış." });
        }

        var students = await _context.Students
            .Where(s => s.AdvisorID == advisorId) // Danışmana ait öğrenciler
            .Select(s => new
            {
                s.StudentID,
                FullName = $"{s.FirstName} {s.LastName}",
                s.Department,
                s.Email
            })
            .ToListAsync();

        if (!students.Any())
        {
            return NotFound(new { message = "Danışmana ait öğrenci bulunamadı." });
        }

        return Ok(new { students });
    }

        public class AdvisorUpdateModel
        {
            public string FullName { get; set; }
            public string Title { get; set; }
            public string Email { get; set; }
        }

        // Advisor profilini güncelle
        [HttpPut("updateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] AdvisorUpdateModel model)
        {

            var advisor = await _context.Advisors.FindAsync(id);
            if (advisor == null)
            {
                return NotFound();
            }

            // Advisor bilgilerini güncelle
            advisor.FullName = model.FullName;
            advisor.Title = model.Title;
            advisor.Email = model.Email;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla güncellendi." });
        }

    }

}