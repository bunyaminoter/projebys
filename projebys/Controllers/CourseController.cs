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

            // Dersi veritabanından siliyoruz
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            // Başarılı bir şekilde silindiğinde 200 OK döndürüyoruz
            return Ok(new { success = true, message = "Ders başarıyla silindi." });
        }
        // Update course
        [HttpPost("Update")]
        public IActionResult UpdateCourse([FromBody] Courses updatedCourse)
        {
            var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseID == updatedCourse.CourseID);
            if (existingCourse == null)
            {
                return NotFound();
            }

            existingCourse.CourseName = updatedCourse.CourseName;
            existingCourse.CourseCode = updatedCourse.CourseCode;
            existingCourse.Credit = updatedCourse.Credit;
            existingCourse.IsMandatory = updatedCourse.IsMandatory;
            existingCourse.Department = updatedCourse.Department;

            _context.SaveChanges();
            return Ok(new { success = true });
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

        // POST: api/Courses/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddCourse( [FromBody] AddCourseModel course)
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
                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync(); // Veritabanına kaydet
                return Ok(new { success = true, message = "Ders başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Bir hata oluştu.", error = ex.Message });
            }
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
