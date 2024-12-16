using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using projebys.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace projebys.Pages.Student
{
    public class TranscriptModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TranscriptModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public List<CourseGrade> Courses { get; set; }

        public IActionResult OnGet(int id)
        {
            // Öðrenciyi ve transkriptini alýyoruz
            var student = _context.Students
                .Include(s => s.Transcripts)  // Öðrencinin transkriptlerini dahil et
                .ThenInclude(t => t.Course)   // Ders bilgilerini dahil et
                .FirstOrDefault(s => s.StudentID == id);

            if (student == null)
            {
                return RedirectToPage("/Error");
            }

            // Öðrenci bilgilerini alýyoruz
            StudentID = student.StudentID;
            StudentName = student.FirstName + " " + student.LastName;

            // Transcripts içerisinden ders bilgilerini ve notlarý alýyoruz
            Courses = student.Transcripts
                .Select(t => new CourseGrade
                {
                    CourseName = t.Course.CourseName,  // Ders adý (Course'dan alýnýyor)
                    Credit = t.Course.Credit,          // Kredi (Course'dan alýnýyor)
                    Grade = t.Grade                    // Dersin notu (Transcripts'tan alýnýyor)
                }).ToList();

            return Page();
        }

        public class CourseGrade
        {
            public string CourseName { get; set; }
            public int Credit { get; set; }
            public string Grade { get; set; }
        }
    }
}
