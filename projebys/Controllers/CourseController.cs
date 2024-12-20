using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projebys.Data;
using projebys.Models;
using projebys.Pages.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projebys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm dersleri getir
        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseName,
                    c.Department,
                    c.IsMandatory,
                    c.CourseCode,
                    c.Credit
                })
                .ToListAsync();

            return Ok(new { courses });
        }

        // DELETE: api/Courses/Delete/{courseID}
        [HttpDelete("Delete/{courseID}")]
        public async Task<IActionResult> DeleteCourse(int courseID)
        {
            // courseID ile derse ulaşmaya çalışıyoruz
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseID);

            if (course == null)
            {
                // Ders bulunamazsa 404 Not Found döndürüyoruz
                return NotFound(new { message = "Ders bulunamadı." });
            }

            // İlişkili verileri siliyoruz (örneğin CourseQuotas tablosu)
            var relatedQuotas = _context.CourseQuotas.Where(q => q.CourseID == courseID);
            _context.CourseQuotas.RemoveRange(relatedQuotas);

            // Dersi siliyoruz
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            // Başarılı bir şekilde silindiğinde 200 OK döndürüyoruz
            return Ok(new { success = true, message = "Ders başarıyla silindi." });
        }


        // GET: api/Courses/GetCourseById/{courseId}
        [HttpGet("GetCourseById/{courseId}")]
        public async Task<IActionResult> GetCourseById(int courseId)
        {
            var course = await _context.Courses.Where(c => c.CourseID == courseId).FirstOrDefaultAsync();

            //var course = _context.Courses.FirstOrDefault(c => c.CourseID == courseId);

            if (course == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            return Ok(new { course });
        }


        // GET: api/Courses/GetCoursesByClass/{studentId}
        [HttpGet("GetCoursesByClass/{studentId}")]
        public async Task<IActionResult> GetCoursesByClass(int studentId)
        {
            // Öğrenciyi ID'ye göre bul
            var student = await _context.Students
                .Include(s => s.Class) // Öğrencinin sınıfını dahil et
                .FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (student == null)
            {
                return NotFound(new { message = "Öğrenci bulunamadı." });
            }

            // Öğrencinin sınıf kimliği
            var classId = student.ClassID;

            // Sınıfın derslerini getir
            var courses = await _context.ClassCourseMappings
                .Where(mapping => mapping.ClassID == classId) // Sınıfa ait dersler
                .Include(mapping => mapping.Course) // Ders bilgilerini dahil et
                .Select(mapping => new
                {
                    mapping.Course.CourseID,
                    mapping.Course.CourseCode,
                    mapping.Course.CourseName,
                    mapping.Course.Credit,
                    mapping.IsMandatory, // Zorunlu olup olmadığını belirt
                    Quota = mapping.Course.CourseQuotas != null ? mapping.Course.CourseQuotas.Quota : 0,
                    RemainingQuota = mapping.Course.CourseQuotas != null ? mapping.Course.CourseQuotas.RemainingQuota : 0
                })
                .ToListAsync();

            return Ok(new { courses });
        }



        // POST: api/Courses/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddCourse([FromBody] AddCourseModel course)
        {
            // Gelen veriyi doğrula
            if (course == null || string.IsNullOrWhiteSpace(course.CourseName) || string.IsNullOrWhiteSpace(course.CourseCode))
            {
                return BadRequest(new { success = false, message = "Geçersiz ders bilgileri." });
            }

            // Yeni ders kaydı oluştur
            var newCourse = new Courses
            {
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                Credit = course.Credit,
                Department = course.Department,
                IsMandatory = course.IsMandatory
            };

            try
            {
                // Ders veritabanına kaydediliyor
                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync(); // Ders kaydedildikten sonra CourseID oluşur

                // Tek bir başlangıç kotası belirleniyor
                var defaultQuota = 50; // Tüm dersler için başlangıç kotası
                var courseQuota = new CourseQuotas
                {
                    CourseID = newCourse.CourseID,  // Oluşan dersin CourseID'si alınıyor
                    Quota = defaultQuota,
                    RemainingQuota = defaultQuota
                };

                // Kontenjan kaydı oluşturuluyor
                _context.CourseQuotas.Add(courseQuota);
                await _context.SaveChangesAsync(); // Kontenjan kaydediliyor

                return Ok(new { success = true, message = "Ders ve kontenjan başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Bir hata oluştu.", error = ex.Message });
            }
        }


        
        [HttpPut("updateCourseQuota/{courseId}")]
        public async Task<IActionResult> UpdateCourseQuota(int courseId, [FromBody] int newQuota)
        {
            var courseQuota = await _context.CourseQuotas
                .FirstOrDefaultAsync(cq => cq.CourseID == courseId);

            if (courseQuota == null)
            {
                return NotFound(new { message = "Ders için kontenjan bulunamadı." });
            }

            // Kontenjanı güncelle
            courseQuota.Quota = newQuota;
            courseQuota.RemainingQuota = newQuota; // Kontenjanın kaldığı miktarı da sıfırlıyoruz.

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ders kontenjanı başarıyla güncellendi." });
        }

        [HttpGet("getCourseQuota/{courseId}")]
        public async Task<IActionResult> GetCourseQuota(int courseId)
        {
            var courseQuota = await _context.CourseQuotas
                .FirstOrDefaultAsync(cq => cq.CourseID == courseId);

            if (courseQuota == null)
            {
                return NotFound(new { message = "Ders için kontenjan bilgisi bulunamadı." });
            }

            return Ok(new
            {
                courseQuota.Quota,
                courseQuota.RemainingQuota
            });
        }

        [HttpPost("assignCourseToClass/{classId}")]
        public async Task<IActionResult> AssignCourseToClass(int classId, [FromBody] List<int> courseIds)
        {
            var classEntity = await _context.Classes.FindAsync(classId);

            if (classEntity == null)
            {
                return NotFound(new { message = "Sınıf bulunamadı." });
            }

            foreach (var courseId in courseIds)
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course != null)
                {
                    // Sınıf ve ders arasındaki eşlemeyi yapıyoruz
                    var mapping = new ClassCourseMappings
                    {
                        ClassID = classId,
                        CourseID = courseId
                    };

                    _context.ClassCourseMappings.Add(mapping);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ders(ler) başarıyla sınıfa atandı." });
        }


        public class AddCourseModel
         {
            public string CourseCode { get; set; }  // Ders kodu
            public string CourseName { get; set; } // Ders adı
            public bool IsMandatory { get; set; } // Zorunlu ders mi?
            public int Credit { get; set; } // Kredi sayısı
            public string Department { get; set; }  // Bölüm
         }


        // Model sınıfı: Ders güncelleme için
        public class UpdateCourseModel
        {
            public int CourseID { get; set; } // Ders kimliği
            public string CourseCode { get; set; }  // Ders kodu
            public string CourseName { get; set; } // Ders adı
            public bool IsMandatory { get; set; } // Zorunlu ders mi?
            public int Credit { get; set; } // Kredi sayısı
            public string Department { get; set; }  // Bölüm
        }
    }
}
