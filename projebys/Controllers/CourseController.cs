using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projebys.Data;
using projebys.Models;
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


        // Ders bilgilerini güncelle
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseModel model)
        {
            if (model == null || model.CourseID <= 0)
            {
                return BadRequest(new { message = "Geçersiz ders bilgisi gönderildi." });
            }

            var course = await _context.Courses.FindAsync(model.CourseID);
            if (course == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            // Ders bilgilerini güncelle
            course.CourseName = model.CourseName;
            course.CourseCode = model.CourseCode;
            course.Credit = model.Credit;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Ders başarıyla güncellendi." });
        }

    }

    // Model sınıfı: Ders güncelleme için
    public class UpdateCourseModel
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int Credit { get; set; }
    }
}
