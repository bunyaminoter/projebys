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
                user.Advisor.Email,
                user.Advisor.Title
            }
        });
    }

        // Danışmanın öğrencilerini getir
        [HttpGet("GetMyStudents/{id}")]
        public async Task<IActionResult> GetMyStudents(int id)
        {
            var advisor = await _context.Advisors.FindAsync(id);
            if (advisor == null)
            {
                return NotFound(new { message = "Danışman bulunamadı." });
            }

            // Danışmanın öğrencilerini getir
            var students = await _context.Students
                .Where(s => s.AdvisorID == id)  // Danışmana ait öğrenciler
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

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Geçersiz veri gönderildi." });
            }

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


        // Öğrencinin dersini onaylamak için API
        [HttpPost("ApproveStudentCourse/{studentId}/{courseId}")]
        public async Task<IActionResult> ApproveStudentCourse(int studentId, int courseId)
        {
            // Öğrencinin ders kaydını bul
            var courseSelection = await _context.StudentCourseSelections
                .FirstOrDefaultAsync(s => s.StudentID == studentId && s.CourseID == courseId);

            if (courseSelection == null)
            {
                return NotFound("Öğrencinin dersi bulunamadı.");
            }

            // Ders zaten onaylanmış mı? Eğer onaylanmışsa, hata mesajı döneceğiz
            if (courseSelection.IsApproved)
            {
                return BadRequest("Bu ders zaten onaylanmış.");
            }

            // Dersin onay durumunu güncelle
            courseSelection.IsApproved = true;

            await _context.SaveChangesAsync();
            return NoContent(); // Başarılı
        }


        // API: Öğrencinin Seçtiği Dersler ve Onay Durumu
        [HttpGet("GetStudentCourses/{studentId}")]
        public async Task<IActionResult> GetStudentCourses(int studentId)
        {
            var studentCourses = await _context.StudentCourseSelections
                .Where(s => s.StudentID == studentId)
                .Include(s => s.Course)  // Ders bilgilerini de alıyoruz
                .Select(s => new
                {
                    s.CourseID,
                    s.Course.CourseName,
                    s.IsApproved
                })
                .ToListAsync();

            if (studentCourses == null || !studentCourses.Any())
            {
                return NotFound("Öğrencinin dersleri bulunamadı.");
            }

            return Ok(studentCourses);
        }

    }

}
