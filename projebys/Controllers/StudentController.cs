using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projebys.Data;
using projebys.Models;
using System;
using System.Linq;

namespace projebys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public class StudentUpdateModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        // Öğrenci bilgilerini al
        [HttpGet]
        [Route("getStudentInfo")]
        public IActionResult GetStudentInfo()
        {
            // Oturumdaki UserID’yi al
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return Unauthorized(new { message = "Kullanıcı giriş yapmamış." });
            }

            // Kullanıcı ve öğrenci bilgilerini al
            var user = _context.Users
                               .Include(u => u.Student)
                               .FirstOrDefault(u => u.UserID == userId);

            if (user == null || user.Student == null)
            {
                return NotFound(new { message = "Kullanıcı veya öğrenci bilgileri bulunamadı." });
            }

            // Kullanıcı bilgilerini dön
            return Ok(new
            {
                user.UserID,
                user.Username,
                user.Email,
                user.Role,
                studentInfo = new
                {
                    user.Student.StudentID,
                    user.Student.FirstName,
                    user.Student.LastName,
                    user.Student.Department
                }
            });
        }
        // Öğrenci profilini güncelle
        [HttpPut("updateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] StudentUpdateModel model)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Öğrenci bilgilerini güncelle
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Email = model.Email;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla güncellendi." });
        }



        [HttpGet("transcript/{id}")]
        public async Task<IActionResult> GetTranscript(int id)
        {
            // Öğrencinin bilgilerini ve derslerini ID'ye göre alıyoruz
            var student = await _context.Students
                .Include(s => s.CourseSelections)  // Öğrencinin seçtiği dersler
                .ThenInclude(sc => sc.Course)      // Seçilen derslerin detaylarını al
                .FirstOrDefaultAsync(s => s.StudentID == id);  // Öğrencinin ID'sine göre sorgulama yap

            if (student == null)
            {
                return NotFound(new { message = "Öğrenci bulunamadı." });
            }

            // Öğrencinin transkript bilgilerini çekiyoruz
            var transcript = student.CourseSelections
                .Where(sc => sc.IsApproved)  // Sadece onaylı dersleri alıyoruz
                .Select(sc => new
                {
                    sc.Course.CourseName,  // Ders adı
                    sc.Course.Credit,      // Ders kredisi
                    Grade = _context.Transcripts  // Transcripts tablosundan Grade değerini çekiyoruz
                        .Where(t => t.StudentID == id && t.CourseID == sc.Course.CourseID)
                        .Select(t => t.Grade)  // Grade değerini alıyoruz
                        .FirstOrDefault()      // Tek bir sonuç döndürüyoruz
                })
                .ToList();

            // Eğer transkript boşsa, uygun bir mesaj döndürüyoruz
            if (transcript == null || !transcript.Any())
            {
                return NotFound(new { message = "Öğrencinin transkripti bulunamadı." });
            }

            return Ok(transcript);  // Transkripti döndürüyoruz
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetAvailableCourses(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        [HttpPost("selectCourses/{id}")]
        public async Task<IActionResult> SelectedCourses(int id, [FromBody] List<int> courseids)
        {
            // Öğrenciyi ID'ye göre bul
            var student = await _context.Students
                .Include(s => s.CourseSelections)  // Öğrencinin ders seçimlerini dahil et
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                return NotFound(new { message = "Öğrenci bulunamadı." });
            }

            foreach (var courseId in courseids)
            {
                // Dersin var olup olmadığını kontrol et
                var course = await _context.Courses
                    .Include(c => c.CourseQuotas)  // Kontenjan bilgisini dahil et
                    .FirstOrDefaultAsync(c => c.CourseID == courseId);

                if (course != null)
                {
                    // Dersin kontenjanını kontrol et
                    if (course.CourseQuotas != null && course.CourseQuotas.RemainingQuota <= 0)
                    {
                        return BadRequest(new { message = $"Ders '{course.CourseName}' için kontenjan dolmuş." });
                    }

                    // Ders zaten seçilmemişse, yeni bir seçim ekle
                    var existingSelection = student.CourseSelections
                        .FirstOrDefault(sc => sc.CourseID == courseId);

                    if (existingSelection == null)
                    {
                        // Kontenjanı bir azalt
                        course.CourseQuotas.RemainingQuota--;

                        var newSelection = new StudentCourseSelections
                        {
                            StudentID = student.StudentID,
                            CourseID = courseId,
                            SelectionDate = DateTime.Now,
                            IsApproved = false // Dersin onaylanmamış olduğunu varsayalım
                        };

                        student.CourseSelections.Add(newSelection);

                        // Kontenjan değişikliğini kaydet
                        _context.Entry(course).State = EntityState.Modified;
                    }
                    else
                    {
                        // Ders zaten seçilmişse, bir uyarı gönder
                        return BadRequest(new { message = $"Ders '{course.CourseName}' zaten seçildi." });
                    }
                }
                else
                {
                    return NotFound(new { message = $"Ders ID '{courseId}' bulunamadı." });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Dersler başarıyla seçildi." });
        }


        [HttpGet("{studentId}")]
        public async Task<ActionResult<Students>> GetStudentInfo(int studentId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            return Ok(student);
        }


        // Öğrencinin seçtiği dersleri getir
        [HttpGet("MyCourses/{id}")]
        public async Task<IActionResult> GetMyCourses(int id)
        {   
            // Öğrenciyi ve ders seçimlerini al
            var student = await _context.Students
                .Include(s => s.CourseSelections)  // Öğrencinin ders seçimlerini dahil et
                .ThenInclude(sc => sc.Course)      // Dersleri dahil et
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                return NotFound(new { message = "Öğrenci bulunamadı." });
            }

            // Öğrencinin seçtiği dersleri listele
            var courses = student.CourseSelections
                .Select(sc => new
                {
                    sc.Course.CourseName,  // Ders adı
                    sc.Course.Credit,      // Kredi
                    sc.Course.IsMandatory, // Zorunlu olup olmadığı
                    IsApproved = sc.IsApproved ? "Evet" : "Hayır"
                })
                .ToList();

            return Ok(courses);
        }


    }
}
